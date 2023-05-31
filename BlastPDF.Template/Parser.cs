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
                    var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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

    private static bool IsNotDocumentNodeToken(TokenType type)
    {
        return type is not (TokenType.Namespace or TokenType.Import or TokenType.Variable or TokenType.Title or TokenType.Author or TokenType.CreationDate or TokenType.Load or TokenType.Page);
    }
    
    private static bool IsNotPageNodeToken(TokenType type)
    {
        return type is not (TokenType.Width or TokenType.Height or TokenType.Margin or TokenType.Header or TokenType.Body or TokenType.Footer or TokenType.Dpi or TokenType.End);
    }
    
    private static bool IsNotContentNodeToken(TokenType type)
    {
        return type is not TokenType.Text;
    }

    private static (List<Token>, int) ConsumeUntilNextTokenType(IReadOnlyList<Token> tokens, int tokenIndex, Func<TokenType, bool> typePredicate)
    {
        var results = new List<Token>(); 
        while (tokenIndex < tokens.Count && typePredicate(tokens[tokenIndex].Type))
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

        var (errorTokens, finalIndex) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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

        var (errorTokens, finalIndex) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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

        var (errorTokens, finalIndex) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
                return (new DocumentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Found a title statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
                return (new DocumentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Found a creationdate statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
                return (new DocumentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Found a author statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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
            var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
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
        while (tokenIndex < tokens.Count && tokens[tokenIndex].Type != TokenType.End)
        {
            var (pageNode, idx) = ParsePageStatement(tokens, tokenIndex);
            results.Add(pageNode);
            if (tokenIndex == idx)
            {
                tokenIndex = idx + 1;
            }
            else
            {
                tokenIndex = idx;
            }
        }

        if (tokenIndex >= tokens.Count)
        {
            return (new DocumentError
            {
                ErroredTokens = tokens.Skip(startTokenIndex).Take(tokenIndex - startTokenIndex).ToList(),
                Message = "Hit end of document while parsing a page node",
            }, tokenIndex);
        }

        if (tokens[tokenIndex].Type != TokenType.End)
        {
            var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotDocumentNodeToken);
            return (new DocumentError
            {
                ErroredTokens = erroredTokens,
                Message = "Unexpected token. Expected 'end'",
            }, idx);
        }

        return (new AddPageNode
        {
            PageToken = pageToken,
            TypeToken = typeToken,
            PageNodes = results,
            EndToken = tokens[tokenIndex]
        }, tokenIndex + 1);
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
            case TokenType.Dpi:
                return ParseDpiNode(tokens, tokenIndex);
            case TokenType.Margin:
                return ParseMarginNode(tokens, tokenIndex);
            case TokenType.Header:
                return ParseHeaderNode(tokens, tokenIndex);
            case TokenType.Body:
                return ParseBodyNode(tokens, tokenIndex);
            case TokenType.Footer:
                return ParseFooterNode(tokens, tokenIndex);
            default:
            {
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message = $"Unexpected token ({currentToken.Type}, '{currentToken.Lexeme}') expected a page level statement.",
                    Severity = DiagnosticSeverity.Error
                }, idx);
            }
        }
    }

    private static (IPageNode, int) ParseWidthNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // we have already verified this is a width token
        var widthToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { widthToken },
                Message = "Hit end of document while parsing a width node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.Number:
                expr = new NumberValue
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
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message =
                        $"Found a width statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        return (new WidthNode
        {
            WidthToken = widthToken,
            Expression = expr
        }, tokenIndex + 1);
    }
    
    private static (IPageNode, int) ParseHeightNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // we have already verified this is a width token
        var heightToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { heightToken },
                Message = "Hit end of document while parsing a height node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.Number:
                expr = new NumberValue
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
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message =
                        $"Found a height statement but got an ({token.Type}, '{token.Lexeme}') instead of a number or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        return (new HeightNode
        {
            HeightToken = heightToken,
            Expression = expr
        }, tokenIndex + 1);
    }
    
    private static (IPageNode, int) ParseDpiNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // we have already verified this is a dpi token
        var dpiToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { dpiToken },
                Message = "Hit end of document while parsing a dpi node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.Number:
                expr = new NumberValue
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
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message =
                        $"Found a dpi statement but got an ({token.Type}, '{token.Lexeme}') instead of a number or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        return (new DpiNode
        {
            DpiToken = dpiToken,
            Expression = expr
        }, tokenIndex + 1);
    }

    private static (IPageNode, int) ParseMarginNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        // we have already verified this is a margin token
        var marginToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { marginToken },
                Message = "Hit end of document while parsing a margin node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var marginTypeToken = tokens[tokenIndex];
        switch (marginTypeToken.Type)
        {
            case TokenType.Left:
            case TokenType.Right:
            case TokenType.Up:
            case TokenType.Down:
            case TokenType.All:
                break;
            default:
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message =
                        $"Found a margin statement but got an ({marginTypeToken.Type}, '{marginTypeToken.Lexeme}') instead of left, right, up, down, or all",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> {marginToken, marginTypeToken},
                Message = "Hit end of document while parsing a margin node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }
        
        var token = tokens[tokenIndex];
        IExpressionNode expr;
        switch (token.Type)
        {
            case TokenType.Number:
                expr = new NumberValue
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
                var (errorTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotPageNodeToken);
                return (new PageError
                {
                    ErroredTokens = errorTokens,
                    Message =
                        $"Found a margin statement but got an ({token.Type}, '{token.Lexeme}') instead of a number or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        return (new MarginNode
        {
            MarginToken = marginToken,
            MarginTypeToken = marginTypeToken,
            Expression = expr
        }, tokenIndex + 1);
    }

    private static (IPageNode, int) ParseHeaderNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var headerToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { headerToken },
                Message = "Hit end of document while parsing a header node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var (contents, idx) = ParseContentNodes(tokens, tokenIndex);
        tokenIndex = idx;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { headerToken },
                Message = "Hit end of document while parsing a header node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        var endToken = tokens[tokenIndex];
        if (endToken.Type == TokenType.End)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { endToken },
                Message = $"Unexpected token ({endToken.Type}, '{endToken.Lexeme}'). Expected end token",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        return (new HeaderNode
        {
            HeaderToken = headerToken,
            Contents = contents,
            EndToken = endToken
        }, tokenIndex + 1);
    }
    
    private static (IPageNode, int) ParseBodyNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var bodyToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { bodyToken },
                Message = "Hit end of document while parsing a body node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }

        var (contents, idx) = ParseContentNodes(tokens, tokenIndex);
        tokenIndex = idx;
        if (tokenIndex >= tokens.Count)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { bodyToken },
                Message = "Hit end of document while parsing a body node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        var endToken = tokens[tokenIndex];
        if (endToken.Type == TokenType.End)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { endToken },
                Message = $"Unexpected token ({endToken.Type}, '{endToken.Lexeme}'). Expected end token",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }

        return (new BodyNode
        {
            BodyToken = bodyToken,
            Contents = contents,
            EndToken = endToken
        }, tokenIndex + 1);
    }

    private static (IPageNode, int) ParseFooterNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var footerToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        { 
            return (new PageError
            {
                ErroredTokens = new List<Token> { footerToken },
                Message = "Hit end of document while parsing a footer node",
                Severity = DiagnosticSeverity.Error,
            }, tokenIndex);
        }
        
        var (contents, idx) = ParseContentNodes(tokens, tokenIndex);
        tokenIndex = idx;
        if (tokenIndex >= tokens.Count)
        { 
            return (new PageError
            { 
                ErroredTokens = new List<Token> { footerToken },
                Message = "Hit end of document while parsing a footer node",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }
        
        var endToken = tokens[tokenIndex];
        if (endToken.Type == TokenType.End)
        {
            return (new PageError
            {
                ErroredTokens = new List<Token> { endToken },
                Message = $"Unexpected token ({endToken.Type}, '{endToken.Lexeme}'). Expected end token",
                Severity = DiagnosticSeverity.Error
            }, tokenIndex);
        }
        
        return (new FooterNode
        {
            FooterToken = footerToken,
            Contents = contents,
            EndToken = endToken
        }, tokenIndex + 1);
    }

    private static (List<IContentNode>, int) ParseContentNodes(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var headerContents = new List<IContentNode>();
        while (tokenIndex < tokens.Count && tokens[tokenIndex].Type == TokenType.End)
        {
            var (node, idx) = tokens[tokenIndex].Type switch
            {
                TokenType.Text => ParseTextNode(tokens, tokenIndex),
                _ => (null, -1),
            };

            if (idx == -1)
            {
                var (erroredTokens, realidx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotContentNodeToken);
                headerContents.Add(new ContentError
                {
                    ErroredTokens = erroredTokens,
                    Message = $"Unexpected token ({tokens[tokenIndex].Type}, '{tokens[tokenIndex].Lexeme}'). Expected a content type node",
                    Severity = DiagnosticSeverity.Error
                });
                tokenIndex = realidx;
                continue;
            }
            
            headerContents.Add(node);
            tokenIndex = idx;
        }

        return (headerContents, tokenIndex);
    }

    private static (IContentNode, int) ParseTextNode(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var textToken = tokens[tokenIndex];
        tokenIndex += 1;
        if (tokenIndex >= tokens.Count)
        { 
            return (new ContentError
            {
                ErroredTokens = new List<Token> { textToken },
                Message = "Hit end of document while parsing a text node",
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
                var (erroredTokens, idx) = ConsumeUntilNextTokenType(tokens, tokenIndex, IsNotContentNodeToken);
                return (new ContentError
                {
                    ErroredTokens = erroredTokens,
                    Message =
                        $"Found a text statement but got an ({token.Type}, '{token.Lexeme}') instead of a string or an embedded expression",
                    Severity = DiagnosticSeverity.Error
                }, idx);
        }

        return (new TextNode
        {
            TextToken = textToken,
            Expression = expr
        }, tokenIndex + 1);
    }
}