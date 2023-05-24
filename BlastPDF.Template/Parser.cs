using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlastPDF.Template;

public class Parser
{
    public static List<IDocumentNode> Parse(string content)
    {
        var lexer = new Lexer(content);
        var meaningfulTokens = lexer
            .Where(x => x.Type is not TokenType.Unknown and not TokenType.Comment and not TokenType.Eof)
            .ToList();

        return ParseDocumentStatements(meaningfulTokens);
    }

    private static List<IDocumentNode> ParseDocumentStatements(IReadOnlyList<Token> tokens)
    {
        var tokenIndex = 0;
        var results = new List<IDocumentNode>();
        while (tokenIndex < tokens.Count)
        {
            var currentToken = tokens[tokenIndex];
            switch (currentToken.Type)
            {
                case TokenType.Namespace:
                {
                    var (node, idx) = ParseNamespaceNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.Import:
                {
                    var (node, idx) = ParseImportNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.Variable:
                {
                    var (node, idx) = ParseVariableNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                default:
                {
                    var (errorTokens, idx) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
                    results.Add(new DocumentError
                    {
                        ErroredTokens = errorTokens,
                        Message = $"Unexpected token ({currentToken.Type}, '{currentToken.Lexeme}') expected a document level statement.",
                        Severity = DiagnosticSeverity.Error
                    });
                    if (tokenIndex == idx)
                    {
                        tokenIndex = idx + 1;
                    }
                    else
                    {
                        tokenIndex = idx;
                    }
                    break;
                }
            }
        }

        return results;
    }

    private static bool IsDocumentNodeToken(TokenType type)
    {
        return type is not (TokenType.Namespace or TokenType.Import);
    }

    private static (List<Token>, int) ConsumeUntilNextDocumentNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var results = new List<Token>(); 
        while (tokenIndex < tokens.Count && IsDocumentNodeToken(tokens[tokenIndex].Type))
        {
            results.Add(tokens[tokenIndex]);
            tokenIndex += 1;
        }
        return (results, tokenIndex);
    }

    private static (IDocumentNode, int) ParseNamespaceNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the namespace node already
        var namespaceToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { namespaceToken },
                Message = "Hit end of document while parsing a namespace node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var nextToken = tokens[tokenIndex];
        if (nextToken.Type == TokenType.EmbeddedExpression)
        {
            return (new NamespaceNode
            {
                NamespaceToken = namespaceToken,
                Value = nextToken
            }, tokenIndex + 1);
        }

        var (errorTokens, finalIndex) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
        return (new DocumentError
        {
            ErroredTokens = errorTokens,
            Message = $"Expected an embedded expression but got ({nextToken.Type}, '{nextToken.Lexeme}')",
            Severity = DiagnosticSeverity.Error
        }, finalIndex);
    }

    private static (IDocumentNode, int) ParseImportNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the namespace node already
        var importToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { importToken },
                Message = "Hit end of document while parsing an import node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var nextToken = tokens[tokenIndex];
        if (nextToken.Type == TokenType.EmbeddedExpression)
        {
            return (new ImportNode
            {
                ImportToken = importToken,
                Value = nextToken
            }, tokenIndex + 1);
        }

        var (errorTokens, finalIndex) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
        return (new DocumentError
        {
            ErroredTokens = errorTokens,
            Message = $"Expected an embedded expression but got ({nextToken.Type}, '{nextToken.Lexeme}')",
            Severity = DiagnosticSeverity.Error
        }, finalIndex);
    }
    
    private static (IDocumentNode, int) ParseVariableNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the namespace node already
        var varToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { varToken },
                Message = "Hit end of document while parsing an variable node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var nextToken = tokens[tokenIndex];
        if (nextToken.Type == TokenType.EmbeddedExpression)
        {
            return (new ImportNode
            {
                ImportToken = varToken,
                Value = nextToken
            }, tokenIndex + 1);
        }

        var (errorTokens, finalIndex) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
        return (new DocumentError
        {
            ErroredTokens = errorTokens,
            Message = $"Expected an embedded expression but got ({nextToken.Type}, '{nextToken.Lexeme}')",
            Severity = DiagnosticSeverity.Error
        }, finalIndex);
    }

}