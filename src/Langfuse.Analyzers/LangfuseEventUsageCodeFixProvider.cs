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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(LangfuseEventUsageCodeFixProvider))]
[Shared]
public class LangfuseEventUsageCodeFixProvider : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(
            LangfuseEventUsageAnalyzer.CreateEventInUsingDiagnosticId,
            LangfuseEventUsageAnalyzer.CreateScopedEventWithoutUsingDiagnosticId);

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

        if (diagnostic.Id == LangfuseEventUsageAnalyzer.CreateEventInUsingDiagnosticId)
        {
            // Fix: Replace CreateEvent with CreateScopedEvent
            context.RegisterCodeFix(
                CodeAction.Create(
                    "Change to CreateEventScoped",
                    c => ReplaceMethodNameAsync(
                        context.Document, memberAccess, "CreateEventScoped", c),
                    "ChangeToCreateEventScoped"),
                diagnostic);
        }
        else if (diagnostic.Id == LangfuseEventUsageAnalyzer.CreateScopedEventWithoutUsingDiagnosticId)
        {
            var invocation = memberAccess.Parent as InvocationExpressionSyntax;
            if (invocation != null)
            {
                // Fix 1: Wrap in using statement
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Wrap in using statement",
                        c => WrapInUsingStatementAsync(
                            context.Document, invocation, c),
                        "WrapInUsing"),
                    diagnostic);

                // Fix 2: Change to CreateEvent (if user doesn't want scoped behavior)
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Change to CreateEvent",
                        c => ReplaceMethodNameAsync(
                            context.Document, memberAccess, "CreateEvent", c),
                        "ChangeToCreateEvent"),
                    diagnostic);
            }
        }
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
                var usingKeyword = SyntaxFactory.Token(leadingTrivia, SyntaxKind.UsingKeyword,
                    SyntaxFactory.TriviaList(SyntaxFactory.Space));

                var newLocalDeclaration = localDeclaration
                    .WithLeadingTrivia() // Remove leading trivia from the declaration
                    .WithUsingKeyword(usingKeyword); // Add using keyword with proper trivia

                var newRoot = root.ReplaceNode(localDeclaration, newLocalDeclaration);
                return document.WithSyntaxRoot(newRoot);
            }
        }

        // Handle expression statements (when the invocation is not assigned to a variable)
        if (statement is ExpressionStatementSyntax expressionStatement &&
            expressionStatement.Expression == invocation)
        {
            // Find a suitable variable name
            var variableName = GenerateUniqueVariableName(semanticModel, invocation.SpanStart, "disposable");

            // Create using declaration: using var disposable = invocation;
            var variableDeclaration = SyntaxFactory.VariableDeclaration(
                    SyntaxFactory.IdentifierName("var"))
                .WithVariables(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(variableName))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(invocation))));

            var leadingTrivia = statement.GetLeadingTrivia();
            var usingKeyword = SyntaxFactory.Token(leadingTrivia, SyntaxKind.UsingKeyword,
                SyntaxFactory.TriviaList(SyntaxFactory.Space));

            var usingDeclaration = SyntaxFactory.LocalDeclarationStatement(variableDeclaration)
                .WithLeadingTrivia() // Remove leading trivia from the declaration
                .WithUsingKeyword(usingKeyword) // Add using keyword with proper trivia
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            var newRoot = root.ReplaceNode(statement, usingDeclaration);
            return document.WithSyntaxRoot(newRoot);
        }

        // Find the parent block to get subsequent statements
        var parentBlock = statement.Parent as BlockSyntax;
        if (parentBlock != null)
        {
            var statementIndex = parentBlock.Statements.IndexOf(statement);
            IEnumerable<StatementSyntax> subsequentStatements = parentBlock.Statements.Skip(statementIndex + 1);

            // Create using statement with expression form and wrap subsequent statements
            var usingStatement = SyntaxFactory.UsingStatement(
                    null,
                    invocation,
                    SyntaxFactory.Block(subsequentStatements))
                .WithLeadingTrivia(statement.GetLeadingTrivia());

            // Remove the original statement and subsequent statements, replace with using statement
            IEnumerable<StatementSyntax> newStatements = parentBlock.Statements.Take(statementIndex)
                .Concat(new[] { usingStatement });

            var newBlock = parentBlock.WithStatements(SyntaxFactory.List(newStatements));
            var newRoot = root.ReplaceNode(parentBlock, newBlock);
            return document.WithSyntaxRoot(newRoot);
        }
        else
        {
            // Fallback: just wrap the current statement
            var usingStatement = SyntaxFactory.UsingStatement(
                    null,
                    invocation,
                    SyntaxFactory.Block())
                .WithLeadingTrivia(statement.GetLeadingTrivia());

            var newRoot = root.ReplaceNode(statement, usingStatement);
            return document.WithSyntaxRoot(newRoot);
        }
    }

    private string GenerateUniqueVariableName(SemanticModel semanticModel, int position, string baseName)
    {
        ImmutableArray<ISymbol> symbols = semanticModel.LookupSymbols(position);
        ImmutableHashSet<string> existingNames = symbols.Select(s => s.Name).ToImmutableHashSet();

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