using System.Collections;
using System.Collections.Generic;

namespace BlastPDF.Template;

public class Lexer : IEnumerable<Token>
{
    private readonly string _source;

    public Lexer(string source)
    {
        _source = source;
    }
    
    public IEnumerator<Token> GetEnumerator()
    {
        return new TokenEnumerator(_source);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new TokenEnumerator(_source);
    }
}

public class TokenEnumerator : IEnumerator<Token>
{
    private readonly string _source;
    private int _sourcePosition { get; set; }
    private int _currentLineNumber { get; set; }
    private int _currentColumnNumber { get; set; }
    private int _position { get; set; }
    private List<Token> _tokens { get; set; }

    public TokenEnumerator(string source)
    {
        _source = source;
        _tokens = new List<Token>();
        _position = -1;
        _sourcePosition = 0;
        _currentLineNumber = 1;
        _currentColumnNumber = 0;
    }
    
    public bool MoveNext()
    {
        if (_position + 1 < _tokens.Count)
        {
            _position += 1;
            return true;
        }

        // Is this necessary??
        if (_sourcePosition >= _source.Length)
        {
            return false;
        }
        
        // lex the next token
        _position += 1;
        
        var startLineNumber = _currentLineNumber;
        var startColumnNumber = _currentColumnNumber;
        var startSourceOffset = _sourcePosition;
        var lexeme = "";
        var type = TokenType.Unknown;

        var c = _source[startSourceOffset];
        switch (c)
        {
            case '{':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = "{";
                type = TokenType.LeftCurly;
                break;
            case '}':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = "}";
                type = TokenType.RightCurly;
                break;
            case '(':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = "(";
                type = TokenType.LeftParen;
                break;
            case ')':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = ")";
                type = TokenType.RightParen;
                break;
            case '/':
                // comments
                break;
            case '@':
                // Embedded Expression
                break;
            case '"':
                // string or multiline string
            case (>= 'a' and <= 'z') or (>= 'A' and <= 'Z') or '_':
                break;
            case >= '0' and <= '9' or '-':
                break;
            default:
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = $"{_source[startSourceOffset]}";
                type = TokenType.Unknown;
                break;
        }
        
        _tokens.Add(new Token
        {
            Lexeme = lexeme,
            Type = type,
            Column = (startColumnNumber, _currentColumnNumber),
            Line = (startLineNumber, _currentLineNumber),
            Offset = (startSourceOffset, _sourcePosition)
        });

        return _sourcePosition < _source.Length;
    }

    public void Reset()
    {
        _position = -1;
    }

    public Token Current => _tokens[_position];

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}