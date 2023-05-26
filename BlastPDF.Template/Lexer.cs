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
        _currentColumnNumber = 1;
    }
    
    public bool MoveNext()
    {
        if (_position + 1 < _tokens.Count)
        {
            _position += 1;
            return true;
        }

        if (_sourcePosition >= _source.Length)
        {
            return false;
        }

        // lex the next token
        _position += 1;
        
        // consume whitespace
        while (_sourcePosition < _source.Length && _source[_sourcePosition] is ' ' or '\n' or '\t' or '\r')
        {
            if (_source[_sourcePosition] is '\n')
            {
                _sourcePosition += 1;
                _currentLineNumber += 1;
                _currentColumnNumber = 1;
            }
            else
            {
                _sourcePosition += 1;
                _currentColumnNumber += 1;
            }
        }
        
        // store where we are starting to calculate the ranges
        var startLineNumber = _currentLineNumber;
        var startColumnNumber = _currentColumnNumber;
        var startSourceOffset = _sourcePosition;
        var lexeme = "";
        TokenType type;

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
            case ':':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = ":";
                type = TokenType.Colon;
                break;
            case '/':
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = "/";
                if (_sourcePosition >= _source.Length || _source[_sourcePosition] != '/')
                {
                    type = TokenType.Unknown;
                    break;
                }
                
                // consume second slash because we confirmed it was there with the previous if statement
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                lexeme += "/";
                while (_sourcePosition < _source.Length && _source[_sourcePosition] is not '\n')
                {
                    lexeme += _source[_sourcePosition];
                    _sourcePosition += 1;
                    _currentColumnNumber += 1;
                }

                type = TokenType.Comment;
                break;
            case '@':
                // Embedded Expression
                _currentColumnNumber += 1;
                _sourcePosition += 1;
                lexeme = "@";
                if (_sourcePosition >= _source.Length || _source[_sourcePosition] is not '{')
                {
                    type = TokenType.Unknown;
                    break;
                }
                
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                lexeme += "{";
                while (_sourcePosition < _source.Length && _source[_sourcePosition] is not '}')
                {
                    lexeme += _source[_sourcePosition];
                    if (_source[_sourcePosition] is '\n')
                    {
                        _sourcePosition += 1;
                        _currentLineNumber += 1;
                        _currentColumnNumber = 0;
                    }
                    else
                    {
                        _sourcePosition += 1;
                        _currentColumnNumber += 1;
                    }
                }

                if (_source[_sourcePosition] == '}')
                {
                    _sourcePosition += 1;
                    _currentColumnNumber += 1;
                    lexeme += '}';
                    type = TokenType.EmbeddedExpression;
                }
                else
                {
                    // TODO error unended expression
                    throw new Exception("LEXER ERROR");
                }
                break;
            case '"' or '\'':
                var marker = _source[_sourcePosition];
                lexeme += _source[_sourcePosition];
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                while (_sourcePosition < _source.Length && _source[_sourcePosition] != marker)
                {
                    lexeme += _source[_sourcePosition];
                    if (_source[_sourcePosition] is '\n')
                    {
                        _sourcePosition += 1;
                        _currentLineNumber += 1;
                        _currentColumnNumber = 0;
                    }
                    else
                    {
                        _sourcePosition += 1;
                        _currentColumnNumber += 1;
                    }
                }
                
                if (_source[_sourcePosition] == marker)
                {
                    _sourcePosition += 1;
                    _currentColumnNumber += 1;
                    lexeme += marker;
                    type = TokenType.String;
                }
                else
                {
                    // TODO error unending string
                    throw new Exception("LEXER ERROR");
                }
                break;
            case '%':
                lexeme += _source[_sourcePosition];
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                type = TokenType.Percent;
                break;
            case >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_':
                lexeme += _source[_sourcePosition];
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                while (_sourcePosition < _source.Length && _source[_sourcePosition] is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_')
                {
                    lexeme += _source[_sourcePosition];
                    _sourcePosition += 1;
                    _currentColumnNumber += 1;
                }

                type = lexeme.ToLower() switch
                {
                    "namespace" => TokenType.Namespace,
                    "import" => TokenType.Import,
                    "variable" => TokenType.Variable,
                    "title" => TokenType.Title,
                    "creationdate" => TokenType.CreationDate,
                    "author" => TokenType.Author,
                    "load" => TokenType.Load,
                    "font" => TokenType.Font,
                    "image" => TokenType.Image,
                    "size" => TokenType.Size,
                    "name" => TokenType.Name,
                    "dot" => TokenType.Dot,
                    "inch" => TokenType.Inch,
                    "percent" => TokenType.Percent,
                    "page" => TokenType.Page,
                    "single" => TokenType.Single,
                    "layout" => TokenType.Layout,
                    "header" => TokenType.Header,
                    "body" => TokenType.Body,
                    "footer" => TokenType.Footer,
                    "dpi" => TokenType.Dpi,
                    "width" => TokenType.Width,
                    "height" => TokenType.Height,
                    "margin" => TokenType.Margin,
                    "left" => TokenType.Left,
                    "right" => TokenType.Right,
                    "up" => TokenType.Up,
                    "down" => TokenType.Down,
                    "all" => TokenType.All,
                    "text" => TokenType.Text,
                    "rise" => TokenType.Rise,
                    "space" => TokenType.Space,
                    "word" => TokenType.Word,
                    "char" => TokenType.Char,
                    "leading" => TokenType.Leading,
                    "transform" => TokenType.Transform,
                    "offset" => TokenType.Offset,
                    "translate" => TokenType.Translate,
                    "scale" => TokenType.Scale,
                    "rotate" => TokenType.Rotate,
                    "skew" => TokenType.Skew,
                    "rendermode" => TokenType.RenderMode,
                    "fill" => TokenType.Fill,
                    "stroke" => TokenType.Stroke,
                    "fillstroke" => TokenType.FillStroke,
                    "invisible" => TokenType.Invisible,
                    "fillclip" => TokenType.FillClip,
                    "strokeclip" => TokenType.StrokeClip,
                    "fillstrokeclip" => TokenType.FillStrokeClip,
                    "clip" => TokenType.Clip,
                    "if" => TokenType.If,
                    "then" => TokenType.Then,
                    "else" => TokenType.Else,
                    "loop" => TokenType.Loop,
                    "in" => TokenType.In,
                    "end" => TokenType.End,
                    _ => TokenType.Identifier
                };

                break;
            case >= '0' and <= '9' or '-':
                lexeme += _source[_sourcePosition];
                _sourcePosition += 1;
                _currentColumnNumber += 1;
                var foundDecimal = false;
                while (_sourcePosition < _source.Length && _source[_sourcePosition] is >= '0' and <= '9' or '.')
                {
                    if (_source[_sourcePosition] == '.')
                    {
                        if (foundDecimal) break;
                        foundDecimal = true;
                    }
                    lexeme += _source[_sourcePosition];
                    _sourcePosition += 1;
                    _currentColumnNumber += 1;
                }
                type = TokenType.Number;
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
        
        // consume whitespace
        while (_sourcePosition < _source.Length && _source[_sourcePosition] is ' ' or '\n' or '\t' or '\r')
        {
            if (_source[_sourcePosition] is '\n')
            {
                _sourcePosition += 1;
                _currentLineNumber += 1;
                _currentColumnNumber = 1;
            }
            else
            {
                _sourcePosition += 1;
                _currentColumnNumber += 1;
            }
        }

        if (_sourcePosition >= _source.Length)
        {
            _tokens.Add(new Token
            {
                Lexeme = "",
                Type = TokenType.Eof,
                Line = (_currentLineNumber, _currentLineNumber),
                Column = (_currentColumnNumber, _currentColumnNumber),
                Offset = (_sourcePosition, _sourcePosition),
            });
        }
        
        return true;
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