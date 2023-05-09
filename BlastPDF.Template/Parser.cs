using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace BlastPDF.Template;

public class Parser
{
    public static List<IValue> Parse(string content)
    {
        var lexer = new Lexer(content);
        var meaningfulTokens = lexer
            .Where(x => x.Type is not TokenType.Unknown and not TokenType.Comment and not TokenType.Eof)
            .ToList();

        var results = new List<IValue>();
        var tokenIndex = 0;
        while (tokenIndex < meaningfulTokens.Count)
        {
            var (value, nextIndex) = ParseValue(meaningfulTokens, tokenIndex);
            tokenIndex = nextIndex;
            results.Add(value);
        }

        return results;
    }

    private static (IValue, int) ParseValue(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        return tokens[tokenIndex].Type switch
        {
            TokenType.Identifier => ParseObjectOrIdentifierValue(tokens, tokenIndex),
            TokenType.String or TokenType.Number or TokenType.EmbeddedExpression => (new Literal { Value = tokens[tokenIndex] }, tokenIndex + 1),
            _ => (
                new Error
                {
                    ErrorToken = tokens[tokenIndex],
                    Message = "Unexpected token :(  Expected: Identifier, String, or Embedded Expression.",
                    Severity = DiagnosticSeverity.Error
                }, tokenIndex + 1)
        };
    }

    private static (IValue, int) ParseObjectOrIdentifierValue(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        if (tokenIndex == tokens.Count - 1) return (new Literal { Value = tokens[tokenIndex] }, tokenIndex + 1);

        return tokens[tokenIndex + 1].Type switch
        {
            TokenType.LeftParen or TokenType.LeftCurly => ParseObject(tokens, tokenIndex),
            _ => (new Literal { Value = tokens[tokenIndex] }, tokenIndex + 1)
        };
    }

    private static (IValue, int) ParseObject(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var identifier = tokens[tokenIndex];
        Debug.WriteLine($"BEFORE ARGUMENT LIST {identifier.Lexeme}");
        var (argumentList, nextIndex) = ParseArgumentList(tokens, tokenIndex + 1, true);
        Debug.WriteLine($"AFTER ARGUMENT LIST {identifier.Lexeme}");
        var (objectBody, finalIndex) = ParseObjectBody(tokens, nextIndex);
        return (new Object
        {
            Name = identifier,
            ArgumentList = argumentList,
            Body = objectBody
        }, finalIndex);
    }
    
    private static (Token, int) ParseTokenByType(IReadOnlyList<Token> tokens, int tokenIndex, TokenType type)
    {
        // TODO remove these exceptions and return diagnostic errors
        if (tokenIndex >= tokens.Count)
        {
            throw new Exception($"Unexpected end of tokens {type}");
        }
        
        // TODO remove these exceptions and return diagnostic errors
        if (tokens[tokenIndex].Type != type)
        {
            throw new Exception($"Unexpected token :( Expected {type} got '{tokens[tokenIndex].Lexeme}' Line: {tokens[tokenIndex].Line.Item1} Column: {tokens[tokenIndex].Column.Item1}");
        }

        return (tokens[tokenIndex], tokenIndex + 1);
    }

    private static (ArgumentVector, int) ParseArgumentList(IReadOnlyList<Token> tokens, int tokenIndex, bool optional)
    {
        // TODO remove these exceptions and return diagnostic errors
        if (tokenIndex >= tokens.Count) throw new Exception("Unexpected end of tokens :(");

        if (tokens[tokenIndex].Type != TokenType.LeftParen)
        {
            Debug.WriteLine(optional ? "OPTIONAL!!!" : "Not optional");
            // TODO fix this
            return optional ? (null, tokenIndex) : throw new Exception($"Unexpected token :( arg list '{tokens[tokenIndex].Lexeme}' Line: {tokens[tokenIndex].Line.Item1} Column: {tokens[tokenIndex].Column.Item1}");
        }

        var openParen = tokens[tokenIndex];
        var currentTokenIndex = tokenIndex + 1;

        var arguments = new List<IArgumentValue>();
        while (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].Type != TokenType.RightParen)
        {
            var (value, nextIndex) = ParseArgument(tokens, currentTokenIndex);
            if (nextIndex < tokens.Count && tokens[nextIndex].Type == TokenType.Comma)
            {
                currentTokenIndex = nextIndex + 1;
            }
            else
            {
                currentTokenIndex = nextIndex;
            }
            arguments.Add(value);
        }

        var (closeParen, finalIndex) = ParseTokenByType(tokens, currentTokenIndex, TokenType.RightParen);

        return (new ArgumentVector
        {
            ArgumentValues = arguments,
            OpenParen = openParen,
            CloseParen = closeParen,
            Colon = null,
            Name = null,
        }, finalIndex);
    }

    private static (ObjectBody, int) ParseObjectBody(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var (openCurly, currentTokenIndex) = ParseTokenByType(tokens, tokenIndex, TokenType.LeftCurly);

        var arguments = new List<IValue>();
        while (currentTokenIndex < tokens.Count && tokens[currentTokenIndex].Type != TokenType.RightCurly)
        {
            var (value, nextIndex) = ParseValue(tokens, currentTokenIndex);
            currentTokenIndex = nextIndex;
            arguments.Add(value);
        }

        var (closeCurly, finalIndex) = ParseTokenByType(tokens, currentTokenIndex, TokenType.RightCurly);

        return (new ObjectBody
        {
            Values = arguments,
            OpenBrace = openCurly,
            CloseBrace = closeCurly,
        }, finalIndex);
    }

    

    private static (IArgumentValue, int) ParseArgument(IReadOnlyList<Token> tokens, int tokenIndex)
    {
        var (identifier, nextIndex) = ParseTokenByType(tokens, tokenIndex, TokenType.Identifier);
        var (colon, nextNextIndex) = ParseTokenByType(tokens, nextIndex, TokenType.Colon);

        if (nextNextIndex >= tokens.Count)
        {
            // TODO remove these exceptions and report diagnostic errors
            throw new Exception("Unexpected end of token input. ARGUMENT");
        }

        return tokens[nextNextIndex].Type switch
        {
            TokenType.EmbeddedExpression or TokenType.String or TokenType.Number or TokenType.Identifier => (
                new ArgumentScalar { Name = identifier, Colon = colon, Value = tokens[nextNextIndex] },
                nextNextIndex + 1),
            TokenType.LeftParen => ParseArgumentList(tokens, nextNextIndex, false),
            // TODO remove these exceptions and report diagnostic errors
            _ => throw new Exception($"Unexpected argument token :( '{tokens[nextNextIndex].Lexeme}' Line: {tokens[nextNextIndex].Line.Item1} Column: {tokens[nextNextIndex].Column.Item1}")
        };
    }
}