using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BlastPDF.Template;

public class Parser
{
    public static IEnumerable<IDocumentNode> Parse(string content)
    {
        var lexer = new Lexer(content);
        var meaningfulTokens = lexer
            .Where(x => x.Type is not TokenType.Unknown and not TokenType.Comment and not TokenType.Eof)
            .ToList();

        return ParseDocumentStatements(meaningfulTokens);
    }

    private static IEnumerable<IDocumentNode> ParseDocumentStatements(IReadOnlyList<Token> tokens)
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
                case TokenType.Title:
                {
                    var (node, idx) = ParseTitleNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.CreationDate:
                {
                    var (node, idx) = ParseCreationNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.Author:
                {
                    var (node, idx) = ParseAuthorNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.Load:
                {
                    var (node, idx) = ParseLoadNode(tokens, tokenIndex);
                    results.Add(node);
                    tokenIndex = idx;
                    break;
                }
                case TokenType.Page:
                {
                    var (node, idx) = ParsePageNode(tokens, tokenIndex);
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
        return type is not (TokenType.Namespace or TokenType.Import or TokenType.Variable or TokenType.Title or TokenType.Author or TokenType.CreationDate or TokenType.Load);
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
        // assume we check the namespace token already
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
        // assume we check the import token already
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
        // assume we check the variable token already
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
            return (new VariableNode
            {
                VariableToken = varToken,
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
    
    private static (IDocumentNode, int) ParseTitleNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the title token already
        var titleToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { titleToken },
                Message = "Hit end of document while parsing a title node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.String:
                expr = new StringValue
                {
                    Value = token
                };
                break;
            case TokenType.EmbeddedExpression:
                expr = new ExpressionValue
                {
                    Value = token
                };
                break;
            default:
                return (new DocumentError
                {
                    ErroredTokens = new List<Token> { titleToken },
                    Message =
                        $"Found a title statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, tokenIndex + 1);
        }

        return (new TitleNode
        {
            TitleToken = titleToken,
            Expression = expr
        }, tokenIndex + 1);
    }
    
    private static (IDocumentNode, int) ParseCreationNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the creationdate token already
        var createToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { createToken },
                Message = "Hit end of document while parsing a title node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.String:
                expr = new StringValue
                {
                    Value = token
                };
                break;
            case TokenType.EmbeddedExpression:
                expr = new ExpressionValue
                {
                    Value = token
                };
                break;
            default:
                return (new DocumentError
                {
                    ErroredTokens = new List<Token> { createToken },
                    Message =
                        $"Found a creationdate statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, tokenIndex + 1);
        }

        return (new CreationDateNode
        {
            CreationDateToken = createToken,
            Expression = expr
        }, tokenIndex + 1);
    }
    
    private static (IDocumentNode, int) ParseAuthorNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we check the author token already
        var authorToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { authorToken },
                Message = "Hit end of document while parsing an author node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.String:
                expr = new StringValue
                {
                    Value = token
                };
                break;
            case TokenType.EmbeddedExpression:
                expr = new ExpressionValue
                {
                    Value = token
                };
                break;
            default:
                return (new DocumentError
                {
                    ErroredTokens = new List<Token> { authorToken },
                    Message =
                        $"Found a author statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, tokenIndex + 1);
        }

        return (new AuthorNode
        {
            AuthorToken = authorToken,
            Expression = expr
        }, tokenIndex + 1);
    }

    private static (IDocumentNode, int) ParseLoadNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // assume we checked the load token already
        var loadToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { loadToken },
                Message = "Hit end of document while parsing a load node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        var typeToken = tokens[tokenIndex];
        switch (typeToken.Type)
        {
            case TokenType.Font:
            case TokenType.Image:
                break;
            default:
                var (erroredTokens, idx) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
                return (new DocumentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Unexpected type of load statement. Expected 'font' or 'image' but got ({typeToken.Type}, '{typeToken.Lexeme}')",
                    Severity = DiagnosticSeverity.Error,
                }, idx);
        }
        
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { loadToken, typeToken },
                Message = "Hit end of document while parsing a load node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        var identifierToken = tokens[tokenIndex];
        if (identifierToken.Type != TokenType.Identifier)
        {
            var (erroredTokens, idx) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
            return (new DocumentError
            {
                ErroredTokens = erroredTokens,
                Message =
                    $"Unexpected token. Expected an identifier but got ({identifierToken.Type}, '{identifierToken.Lexeme}')",
                Severity = DiagnosticSeverity.Error,
            }, idx);
        }

        tokenIndex += 1;
        if (tokenIndex >= tokens.Count || !(tokens[tokenIndex].Type == TokenType.EmbeddedExpression || tokens[tokenIndex].Type == TokenType.String))
        {
            return (new LoadNode
            {
                LoadToken = loadToken,
                TypeToken = typeToken,
                IdentifierToken = identifierToken,
                Expression = null
            }, tokenIndex);
        }

        return tokens[tokenIndex].Type switch
        {
            TokenType.EmbeddedExpression => (
                new LoadNode
                {
                    LoadToken = loadToken,
                    TypeToken = typeToken,
                    IdentifierToken = identifierToken,
                    Expression = new ExpressionValue { Value = tokens[tokenIndex] }
                }, tokenIndex + 1),
            TokenType.String => (
                new LoadNode
                {
                    LoadToken = loadToken,
                    TypeToken = typeToken,
                    IdentifierToken = identifierToken,
                    Expression = new StringValue { Value = tokens[tokenIndex] }
                }, tokenIndex + 1),
            _ => (
                new DocumentError
                {
                    ErroredTokens = new List<Token> { loadToken, typeToken, identifierToken, tokens[tokenIndex] },
                    Message = "UNEXPECTED CASE HIT",
                    Severity = DiagnosticSeverity.Error
                }, tokenIndex + 1)
        };
    }

    private static (IDocumentNode, int) ParsePageNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        
    }
}