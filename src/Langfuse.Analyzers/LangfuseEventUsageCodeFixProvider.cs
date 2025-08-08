using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Langfuse.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AttributeOnlyLangfuseCodeFixProvider))]
[Shared]
public class AttributeOnlyLangfuseCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            AttributeOnlyLangfuseAnalyzer.NonScopedInUsingDiagnosticId,
            AttributeOnlyLangfuseAnalyzer.ScopedWithoutUsingDiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the member access expression
        var memberAccess = root.FindNode(diagnosticSpan)
            .AncestorsAndSelf()
            .OfType<MemberAccessExpressionSyntax>()
            .FirstOrDefault();

        if (memberAccess == null)
        {
            return;
        }

        var invocation = memberAccess.Parent as InvocationExpressionSyntax;
        if (invocation == null)
        {
            return;
        }

        var semanticModel =
            await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        var methodSymbol = semanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;
        if (methodSymbol == null)
        {
            return;
        }

        if (diagnostic.Id == AttributeOnlyLangfuseAnalyzer.NonScopedInUsingDiagnosticId)
        {
            // Get the scoped variant name from the attribute
            var scopedMethodName = GetScopedVariantFromAttribute(methodSymbol);
            if (!string.IsNullOrEmpty(scopedMethodName))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Change to {scopedMethodName}",
                        c => ReplaceMethodNameAsync(
                            context.Document, memberAccess, scopedMethodName, c),
                        $"ChangeToScoped_{scopedMethodName}"),
                    diagnostic);
            }
        }
        else if (diagnostic.Id == AttributeOnlyLangfuseAnalyzer.ScopedWithoutUsingDiagnosticId)
        {
            // Fix 1: Wrap in using statement
            context.RegisterCodeFix(
                CodeAction.Create(
                    "Wrap in using statement",
                    c => WrapInUsingStatementAsync(
                        context.Document, invocation, c),
                    "WrapInUsing"),
                diagnostic);

            // Fix 2: Change to non-scoped variant (if specified in attribute)
            var nonScopedMethodName = GetNonScopedVariantFromAttribute(methodSymbol);
            if (!string.IsNullOrEmpty(nonScopedMethodName))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        $"Change to {nonScopedMethodName}",
                        c => ReplaceMethodNameAsync(
                            context.Document, memberAccess, nonScopedMethodName, c),
                        $"ChangeToNonScoped_{nonScopedMethodName}"),
                    diagnostic);
            }
        }
    }

    private static string GetScopedVariantFromAttribute(IMethodSymbol methodSymbol)
    {
        var attr = methodSymbol.GetAttributes()
            .FirstOrDefault(a =>
                a.AttributeClass?.Name == "NonScopedMethod" ||
                a.AttributeClass?.Name == "NonScopedMethodAttribute");

        if (attr != null && attr.ConstructorArguments.Length > 0)
        {
            return attr.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
        }

        return string.Empty;
    }

    private static string GetNonScopedVariantFromAttribute(IMethodSymbol methodSymbol)
    {
        var attr = methodSymbol.GetAttributes()
            .FirstOrDefault(a =>
                a.AttributeClass?.Name == "ScopedMethod" ||
                a.AttributeClass?.Name == "ScopedMethodAttribute");

        if (attr != null)
        {
            // Check constructor arguments
            if (attr.ConstructorArguments.Length > 0)
            {
                return attr.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
            }

            // Check named arguments
            KeyValuePair<string, TypedConstant> namedArg =
                attr.NamedArguments.FirstOrDefault(na => na.Key == "NonScopedVariant");
            if (namedArg.Key != null)
            {
                return namedArg.Value.Value?.ToString() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    private async Task<Document> ReplaceMethodNameAsync(
        Document document,
        MemberAccessExpressionSyntax memberAccess,
        string newMethodName,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var newMemberAccess = memberAccess.WithName(
            SyntaxFactory.IdentifierName(newMethodName));

        var newRoot = root.ReplaceNode(memberAccess, newMemberAccess);
        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> WrapInUsingStatementAsync(
        Document document,
        InvocationExpressionSyntax invocation,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

        // Find the statement containing this invocation
        var statement = invocation.AncestorsAndSelf().OfType<StatementSyntax>().FirstOrDefault();
        if (statement == null)
        {
            return document;
        }

        // Check if it's already part of a variable declaration
        var variableDeclarator = invocation.Parent?.Parent as VariableDeclaratorSyntax;
        if (variableDeclarator != null)
        {
            var localDeclaration = variableDeclarator.Parent?.Parent as LocalDeclarationStatementSyntax;
            if (localDeclaration != null && !localDeclaration.UsingKeyword.IsKind(SyntaxKind.UsingKeyword))
            {
                // Create new using declaration with proper trivia handling
                var leadingTrivia = localDeclaration.GetLeadingTrivia();
                var trailingTrivia = localDeclaration.GetTrailingTrivia();
                
                // Create using keyword with space after it
                var usingKeyword = SyntaxFactory.Token(SyntaxKind.UsingKeyword)
                    .WithTrailingTrivia(SyntaxFactory.Space);

                var newLocalDeclaration = localDeclaration
                    .WithUsingKeyword(usingKeyword)
                    .WithSemicolonToken(localDeclaration.SemicolonToken.WithTrailingTrivia(trailingTrivia)); // Preserve trailing trivia on semicolon

                var newRootNode = root.ReplaceNode(localDeclaration, newLocalDeclaration);
                return document.WithSyntaxRoot(newRootNode);
            }
        }

        // Handle expression statements (when the invocation is not assigned to a variable)
        if (statement is ExpressionStatementSyntax expressionStatement &&
            expressionStatement.Expression == invocation)
        {
            // Get variable name from method
            var methodName = (invocation.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.Text ??
                             "disposable";
            var baseVariableName = GetVariableNameFromMethod(methodName);
            var variableName = GenerateUniqueVariableName(semanticModel, statement.SpanStart, baseVariableName);

            // Create using declaration: using var variableName = invocation;
            // We need to preserve trivia properly - the invocation might have trivia attached to it
            var invocationWithTrivia = invocation.WithoutTrivia(); // Remove trivia from invocation temporarily
            
            var variableDeclaration = SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(variableName))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(invocationWithTrivia))));

            var leadingTrivia = statement.GetLeadingTrivia();
            var trailingTrivia = statement.GetTrailingTrivia();
            
            // Create using keyword with space after it
            var usingKeyword = SyntaxFactory.Token(SyntaxKind.UsingKeyword)
                .WithTrailingTrivia(SyntaxFactory.Space);

            var usingDeclaration = SyntaxFactory.LocalDeclarationStatement(variableDeclaration)
                .WithUsingKeyword(usingKeyword)
                .WithLeadingTrivia(leadingTrivia) // Preserve original leading trivia (including comments)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken).WithTrailingTrivia(trailingTrivia)); // Preserve original trailing trivia

            var newRoot = root.ReplaceNode(statement, usingDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        // For other cases, create a using statement with proper variable naming
        var methodNameForVariable = (invocation.Expression as MemberAccessExpressionSyntax)?.Name.Identifier.Text ??
                                    "disposable";
        var baseVarName = GetVariableNameFromMethod(methodNameForVariable);
        var varName = GenerateUniqueVariableName(semanticModel, statement.SpanStart, baseVarName);

        var usingStatement = SyntaxFactory.UsingStatement(
                SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.IdentifierName("var"))
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(varName))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(invocation)))),
                null,
                SyntaxFactory.Block())
            .WithLeadingTrivia(statement.GetLeadingTrivia());

        // Insert the using statement before the current statement and preserve trivia
        var newRootInsert = root.InsertNodesBefore(statement, new[] { usingStatement });
        return document.WithSyntaxRoot(newRootInsert);
    }

    private string GetVariableNameFromMethod(string methodName)
    {
        var name = methodName;

        if (name.StartsWith("Create"))
        {
            name = name.Substring(6);
        }
        else if (name.StartsWith("Begin") || name.StartsWith("Start"))
        {
            name = name.Substring(5);
        }

        if (name.EndsWith("Scoped") && !name.StartsWith("event", StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring(0, name.Length - 6);
        }

        // Convert first letter to lowercase
        if (name.Length > 0)
        {
            name = char.ToLower(name[0]) + name.Substring(1);
        }

        return string.IsNullOrEmpty(name) ? "disposable" : name;
    }

    private string GenerateUniqueVariableName(SemanticModel semanticModel, int position, string baseName)
    {
        ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position);
        HashSet<string> existingNames = symbols.Select(s => s.Name).ToHashSet();

        var name = baseName;
        var counter = 1;
        while (existingNames.Contains(name))
        {
            name = $"{baseName}{counter}";
            counter++;
        }

        return name;
    }
}