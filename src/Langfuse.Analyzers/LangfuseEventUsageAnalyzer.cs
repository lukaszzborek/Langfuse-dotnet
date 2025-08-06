using System.Collections.Generic;
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
public class AttributeOnlyLangfuseAnalyzer : DiagnosticAnalyzer
{
    public const string NonScopedInUsingDiagnosticId = "LANG001";
    public const string ScopedWithoutUsingDiagnosticId = "LANG002";

    // Attribute names (without "Attribute" suffix, as that's how Roslyn sees them)
    private const string NonScopedMethodAttributeName = "NonScopedMethod";
    private const string NonScopedMethodAttributeFullName = "NonScopedMethodAttribute";
    private const string ScopedMethodAttributeName = "ScopedMethod";
    private const string ScopedMethodAttributeFullName = "ScopedMethodAttribute";

    private static readonly DiagnosticDescriptor NonScopedInUsingRule = new(
        NonScopedInUsingDiagnosticId,
        Resources.LANG001_Title,
        Resources.LANG001_Format,
        "Usage",
        DiagnosticSeverity.Error,
        true,
        Resources.LANG001_Description);

    private static readonly DiagnosticDescriptor ScopedWithoutUsingRule = new(
        ScopedWithoutUsingDiagnosticId,
        Resources.LANG002_Title,
        Resources.LANG002_Format,
        "Usage",
        DiagnosticSeverity.Error,
        true,
        Resources.LANG002_Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(NonScopedInUsingRule, ScopedWithoutUsingRule);

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

        // Get the method symbol
        var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation);
        var methodSymbol = symbolInfo.Symbol as IMethodSymbol;
        if (methodSymbol == null)
        {
            return;
        }

        // Check if method has our attributes
        (bool IsNonScoped, bool IsScoped, string ScopedVariant, string NonScopedVariant)? methodInfo =
            GetMethodInfoFromAttributes(methodSymbol);
        if (!methodInfo.HasValue)
        {
            return; // Method doesn't have our attributes, skip it
        }

        // Check if the invocation is within a using statement
        var isInUsing = IsInUsingStatement(invocation);
        var isInUsingDeclaration = IsInUsingDeclaration(invocation);
        var isAssignedToUsingVariable = IsAssignedToUsingVariable(invocation, context.SemanticModel);

        var isProperlyUsedWithUsing = isInUsing || isInUsingDeclaration || isAssignedToUsingVariable;

        // Report diagnostics based on the attribute and usage
        if (methodInfo.Value.IsNonScoped && isProperlyUsedWithUsing)
        {
            // Non-scoped method is used in a using context - this is wrong
            var diagnostic = Diagnostic.Create(
                NonScopedInUsingRule,
                memberAccess.Name.GetLocation(),
                methodInfo.Value.ScopedVariant,
                methodName);
            context.ReportDiagnostic(diagnostic);
        }
        else if (methodInfo.Value.IsScoped && !isProperlyUsedWithUsing)
        {
            // Scoped method is not used in a using context - this is wrong
            var diagnostic = Diagnostic.Create(
                ScopedWithoutUsingRule,
                memberAccess.Name.GetLocation(),
                methodName);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static (bool IsNonScoped, bool IsScoped, string ScopedVariant, string NonScopedVariant)?
        GetMethodInfoFromAttributes(IMethodSymbol methodSymbol)
    {
        // Check for NonScopedMethod attribute
        var nonScopedAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr =>
                attr.AttributeClass?.Name == NonScopedMethodAttributeName ||
                attr.AttributeClass?.Name == NonScopedMethodAttributeFullName);

        if (nonScopedAttr != null)
        {
            // Get the scoped variant from constructor argument
            var scopedVariant = string.Empty;
            if (nonScopedAttr.ConstructorArguments.Length > 0)
            {
                scopedVariant = nonScopedAttr.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
            }

            return (true, false, scopedVariant, methodSymbol.Name);
        }

        // Check for ScopedMethod attribute
        var scopedAttr = methodSymbol.GetAttributes()
            .FirstOrDefault(attr =>
                attr.AttributeClass?.Name == ScopedMethodAttributeName ||
                attr.AttributeClass?.Name == ScopedMethodAttributeFullName);

        if (scopedAttr != null)
        {
            // Get the non-scoped variant from constructor argument or named argument
            var nonScopedVariant = string.Empty;

            // Check constructor arguments
            if (scopedAttr.ConstructorArguments.Length > 0)
            {
                nonScopedVariant = scopedAttr.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
            }
            // Check named arguments
            else
            {
                KeyValuePair<string, TypedConstant> namedArg = scopedAttr.NamedArguments
                    .FirstOrDefault(na => na.Key == "NonScopedVariant");
                if (namedArg.Key != null)
                {
                    nonScopedVariant = namedArg.Value.Value?.ToString() ?? string.Empty;
                }
            }

            return (false, true, methodSymbol.Name, nonScopedVariant);
        }

        // No relevant attributes found
        return null;
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
            if (parent is MethodDeclarationSyntax ||
                parent is LambdaExpressionSyntax ||
                parent is AnonymousMethodExpressionSyntax)
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
            if (parent is LocalDeclarationStatementSyntax localDecl &&
                localDecl.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
            {
                if (localDecl.Declaration.Variables.Any(v => v.Initializer?.Value?.Contains(node) == true))
                {
                    return true;
                }
            }

            if (parent is MethodDeclarationSyntax ||
                parent is LambdaExpressionSyntax ||
                parent is AnonymousMethodExpressionSyntax)
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