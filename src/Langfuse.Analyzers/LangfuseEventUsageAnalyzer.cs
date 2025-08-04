using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using IMethodSymbol = Microsoft.CodeAnalysis.IMethodSymbol;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;
using LocalizableString = Microsoft.CodeAnalysis.LocalizableString;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;
using SyntaxNode = Microsoft.CodeAnalysis.SyntaxNode;

namespace Langfuse.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LangfuseEventUsageAnalyzer : DiagnosticAnalyzer
{
    public const string CreateEventInUsingDiagnosticId = "LANG001";
    public const string CreateScopedEventWithoutUsingDiagnosticId = "LANG002";

    private static readonly LocalizableString CreateEventInUsingTitle =
        "CreateEvent should not be used in using statement";

    private static readonly LocalizableString CreateEventInUsingMessageFormat =
        "Use 'CreateScopedEvent' instead of 'CreateEvent' when creating events in a using statement";

    private static readonly LocalizableString CreateEventInUsingDescription =
        "When creating events within a using statement, use CreateScopedEvent to properly establish parent-child relationships.";

    private static readonly LocalizableString CreateScopedEventWithoutUsingTitle =
        "CreateEventScoped must be used with using statement";

    private static readonly LocalizableString CreateScopedEventWithoutUsingMessageFormat =
        "CreateEventScoped must be used within a using statement or assigned to a using variable";

    private static readonly LocalizableString CreateScopedEventWithoutUsingDescription =
        "CreateEventScoped returns a disposable that must be properly disposed to maintain the parent context stack.";

    private static readonly DiagnosticDescriptor CreateEventInUsingRule = new(
        CreateEventInUsingDiagnosticId,
        CreateEventInUsingTitle,
        CreateEventInUsingMessageFormat,
        "Usage",
        DiagnosticSeverity.Error,
        true,
        CreateEventInUsingDescription);

    private static readonly DiagnosticDescriptor CreateScopedEventWithoutUsingRule = new(
        CreateScopedEventWithoutUsingDiagnosticId,
        CreateScopedEventWithoutUsingTitle,
        CreateScopedEventWithoutUsingMessageFormat,
        "Usage",
        DiagnosticSeverity.Error,
        true,
        CreateScopedEventWithoutUsingDescription);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(CreateEventInUsingRule, CreateScopedEventWithoutUsingRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register for invocation expressions (method calls)
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        // Check if this is a method call we care about
        var memberAccess = invocation.Expression as MemberAccessExpressionSyntax;
        if (memberAccess == null)
        {
            return;
        }

        var methodName = memberAccess.Name.Identifier.Text;
        if (methodName != "CreateEvent" && methodName != "CreateEventScoped")
        {
            return;
        }

        // Get the symbol to verify it's from LangfuseTrace
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol == null || methodSymbol.ContainingType.Name != "LangfuseTrace")
        {
            return;
        }

        // Check if the invocation is within a using statement
        var isInUsing = IsInUsingStatement(invocation);
        var isInUsingDeclaration = IsInUsingDeclaration(invocation);
        var isAssignedToUsingVariable = IsAssignedToUsingVariable(invocation, context.SemanticModel);

        var isProperlyUsedWithUsing = isInUsing || isInUsingDeclaration || isAssignedToUsingVariable;

        if (methodName == "CreateEvent" && isProperlyUsedWithUsing)
        {
            // CreateEvent is used in a using context - this is wrong
            var diagnostic = Diagnostic.Create(
                CreateEventInUsingRule,
                memberAccess.Name.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
        else if (methodName == "CreateEventScoped" && !isProperlyUsedWithUsing)
        {
            // CreateScopedEvent is not used in a using context - this is wrong
            var diagnostic = Diagnostic.Create(
                CreateScopedEventWithoutUsingRule,
                memberAccess.Name.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsInUsingStatement(SyntaxNode node)
    {
        var parent = node.Parent;
        while (parent != null)
        {
            if (parent is UsingStatementSyntax usingStatement)
            {
                // Check if our node is part of the using expression or declaration
                if (usingStatement.Expression?.Contains(node) == true ||
                    usingStatement.Declaration?.Contains(node) == true)
                {
                    return true;
                }
            }

            // Stop searching if we hit a method declaration or lambda
            
            if (parent is MethodDeclarationSyntax || parent is LambdaExpressionSyntax)
            {
                break;
            }

            parent = parent.Parent;
        }

        return false;
    }

    private static bool IsInUsingDeclaration(SyntaxNode node)
    {
        var parent = node.Parent;
        while (parent != null)
        {
            // C# 8.0+ using declarations
            if (parent is LocalDeclarationStatementSyntax localDecl &&
                localDecl.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
            {
                if (localDecl.Declaration.Variables.Any(v => v.Initializer?.Value?.Contains(node) == true))
                {
                    return true;
                }
            }

            // Stop searching if we hit a method declaration or lambda
            if (parent is MethodDeclarationSyntax || parent is LambdaExpressionSyntax)
            {
                break;
            }

            parent = parent.Parent;
        }

        return false;
    }

    private static bool IsAssignedToUsingVariable(InvocationExpressionSyntax invocation, SemanticModel semanticModel)
    {
        // Check if the invocation is part of an assignment
        var assignment = invocation.Parent as AssignmentExpressionSyntax;
        if (assignment?.Right != invocation)
        {
            // Also check if it's part of a variable initializer
            var variableDeclarator = invocation.Parent?.Parent as VariableDeclaratorSyntax;
            if (variableDeclarator?.Initializer?.Value != invocation)
            {
                return false;
            }
        }

        // Get the variable being assigned to
        ISymbol targetSymbol = null;
        if (assignment != null)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(assignment.Left);
            targetSymbol = symbolInfo.Symbol;
        }
        else
        {
            var variableDeclarator = invocation.Parent?.Parent as VariableDeclaratorSyntax;
            if (variableDeclarator != null)
            {
                targetSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator);
            }
        }

        if (targetSymbol is ILocalSymbol localSymbol)
        {
            // Check if this variable is declared with using
            var syntax = localSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            if (syntax?.Parent?.Parent is LocalDeclarationStatementSyntax localDecl)
            {
                return localDecl.UsingKeyword.IsKind(SyntaxKind.UsingKeyword);
            }
        }

        return false;
    }
}