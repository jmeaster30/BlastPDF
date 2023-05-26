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
        return type is not (TokenType.Namespace or TokenType.Import or TokenType.Variable or TokenType.Title or TokenType.Author or TokenType.CreationDate or TokenType.Load or TokenType.Page);
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
        // assume we checked the page token already
        var pageToken = tokens[tokenIndex];
        var startTokenIndex = tokenIndex;
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = new List<Token> { pageToken },
                Message = "Hit end of document while parsing a page node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        var typeToken = tokens[tokenIndex];
        switch (typeToken.Type)
        {
            case TokenType.Single:
            case TokenType.Layout:
                break;
            default:
                var (erroredTokens, idx) = ConsumeUntilNextDocumentNode(tokens, tokenIndex);
                return (new DocumentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Unexpected type of page statement. Expected 'single' or 'layout' but got ({typeToken.Type}, '{typeToken.Lexeme}')",
                    Severity = DiagnosticSeverity.Error,
                }, idx);
        }

        tokenIndex += 1;
        var results = new List<IPageNode>();
        while (tokenIndex < tokens.Count && tokens[tokenIndex].Type == TokenType.End)
        {
            var (pageNode, idx) = ParsePageStatement(tokens, tokenIndex);
            results.Add(pageNode);
            tokenIndex = idx;
        }

        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = tokens.Skip(startTokenIndex).Take(tokenIndex - startTokenIndex).ToList(),
                Message = "Hit end of document while parsing a page node",
            }, tokenIndex);
        }

        return (new AddPageNode
        {
            PageToken = pageToken,
            TypeToken = typeToken,
            PageNodes = results,
            EndToken = tokens[tokenIndex]
        }, tokenIndex + 1);
    }
    
    private static bool IsPageNodeToken(TokenType type)
    {
        return type is not (TokenType.Width or TokenType.Height or TokenType.Margin or TokenType.Header or TokenType.Body or TokenType.Footer or TokenType.Dpi or TokenType.End);
    }

    private static (List<Token>, int) ConsumeUntilNextPageNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var results = new List<Token>(); 
        while (tokenIndex < tokens.Count && IsPageNodeToken(tokens[tokenIndex].Type))
        {
            results.Add(tokens[tokenIndex]);
            tokenIndex += 1;
        }
        return (results, tokenIndex);
    }

    private static (IPageNode, int) ParsePageStatement(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var currentToken = tokens[tokenIndex];
        switch (currentToken.Type)
        {
            case TokenType.Width:
                return ParseWidthNode(tokens, tokenIndex);
            case TokenType.Height:
                return ParseHeightNode(tokens, tokenIndex);
            case TokenType.Margin:
                return ParseMarginNode(tokens, tokenIndex);
            case TokenType.Header:
                return ParseHeaderNode(tokens, tokenIndex);
            case TokenType.Body:
                return ParseBodyNode(tokens, tokenIndex);
            case TokenType.Footer:
                return ParseFooterNode(tokens, tokenIndex);
            case TokenType.Dpi:
                return ParseDpiNode(tokens, tokenIndex);
            default:
            {
                var (errorTokens, idx) = ConsumeUntilNextPageNode(tokens, tokenIndex);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message = $"Unexpected token ({currentToken.Type}, '{currentToken.Lexeme}') expected a page level statement.",
                    Severity = DiagnosticSeverity.Error
                }, idx);
            }
        }
    }
}