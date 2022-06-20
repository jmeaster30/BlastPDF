using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal
{
  public class Lexer : IDisposable
  {
    private readonly Stream _inputStream;
    private Token CurrentToken { get; set; }

    public static Lexer FromFile(string filepath)
    {
      return new Lexer(File.OpenRead(filepath));
    }

    public static Lexer FromString(string source)
    {
      return new Lexer(new MemoryStream(Encoding.UTF8.GetBytes(source)));
    }

    private Lexer(FileStream file)
    {
      _inputStream = file;
    }

    private Lexer(MemoryStream source)
    {
      _inputStream = source;
    }

    public Token GetToken()
    {
      if (CurrentToken != null) return CurrentToken;
      
      var currentByte = _inputStream.ReadByte();

      if (currentByte < 0) return new(TokenType.EOF, " ");

      TokenType type;
      string lexeme = "";
      string errorMessage = "";
      
      switch (currentByte)
      {
        case 0 or 9 or 12 or 32:
          type = TokenType.WHITESPACE;
          lexeme = lexeme.ConcatByte(currentByte);
          lexeme = ConsumeWhile(lexeme, utfByte => utfByte is 0 or 9 or 12 or 32);
          break;
        case 10:
          type = TokenType.EOL;
          lexeme = lexeme.ConcatByte(currentByte);
          break;
        case 13:
        {
          type = TokenType.EOL;
          lexeme = lexeme.ConcatByte(currentByte);

          var nextByte = _inputStream.ReadByte();
          if (nextByte is 10)
          {
            lexeme = lexeme.ConcatByte(nextByte);
          }
          else if (nextByte is not -1)
          {
            _inputStream.Seek(-1, SeekOrigin.Current);
          }
          break;
        }
        case '%':
          type = TokenType.COMMENT;
          lexeme = lexeme.ConcatByte(currentByte);
          lexeme = ConsumeUntil(lexeme, utfByte => utfByte is 10 or 13);
          break;
        case '/':
          type = TokenType.NAME;
          lexeme = lexeme.ConcatByte(currentByte);
          lexeme = ConsumeWhile(lexeme, utfByte => utfByte is >= '!' and <= '~' && IsRegular(utfByte));
          break;
        case '(':
        {
          type = TokenType.LITERAL;
          lexeme = lexeme.ConcatByte(currentByte);
          var depth = 0;
          var escape = false;
          var nextByte = _inputStream.ReadByte();
          while (nextByte is not -1)
          {
            lexeme = lexeme.ConcatByte(nextByte);
            if (nextByte is ')')
            {
              if (escape)
                escape = false;
              else
                depth -= 1;

              if (depth < 0) break;
            }
            else switch (nextByte)
            {
              case '(' when escape:
                escape = false;
                break;
              case '(':
                depth += 1;
                break;
              case '\\':
                escape = !escape;
                break;
              default:
                escape = false;
                break;
            }
            nextByte = _inputStream.ReadByte();
          }

          if (lexeme[^1..] is not ")" || depth >= 0)
          {
            type = TokenType.ERROR;
            errorMessage = "Unclosed string";
          }

          break;
        }
        case '<':
        {
          lexeme = lexeme.ConcatByte(currentByte);
          var nextByte = _inputStream.ReadByte();
          switch (nextByte)
          {
            case '<':
              type = TokenType.DICT_OPEN;
              lexeme = lexeme.ConcatByte(nextByte);
              break;
            case '>':
              type = TokenType.HEX;
              lexeme = lexeme.ConcatByte(nextByte);
              break;
            default:
              type = TokenType.HEX;
              lexeme = lexeme.ConcatByte(nextByte);
              lexeme = ConsumeUntil(lexeme, utfByte => utfByte is '>');
              if (!lexeme[1..].All(x => IsHex(x) || IsWhitespace(x)))
              {
                errorMessage = "Hex string contains invalid characters.";
                type = TokenType.ERROR;
              }
              nextByte = _inputStream.ReadByte(); // consume the > symbol
              if (nextByte is not '>')
              {
                errorMessage = "Unclosed hex string :(";
                type = TokenType.ERROR;
              }
              lexeme = lexeme.ConcatByte(nextByte);
              break;
          }
          break;
        }
        case '>':
        {
          lexeme = lexeme.ConcatByte(currentByte);
          var nextByte = _inputStream.ReadByte();
          if (nextByte is '>')
          {
            type = TokenType.DICT_CLOSE;
            lexeme = lexeme.ConcatByte(nextByte);
          }
          else
          {
            // I don't think we can hit this actually
            type = TokenType.ERROR;
            lexeme = "Found the end of a hex string outside of a hex string.";
          }
          break;
        }
        case '+' or '-':
        {
          bool point;
          lexeme = lexeme.ConcatByte(currentByte);
          var pos = _inputStream.Position;
          (lexeme, point) = ConsumeNumbers(lexeme, false);
          if (point && lexeme.Length == 2 || !point && lexeme.Length == 1)
          {
            _inputStream.Seek(pos, SeekOrigin.Begin);
            lexeme = lexeme[..1];
            type = TokenType.REGULAR;
          }
          else
          {
            type = point ? TokenType.REAL : TokenType.INTEGER;
          }
          break;
        }
        // numbers
        case '.':
          lexeme = lexeme.ConcatByte(currentByte);
          (lexeme, _) = ConsumeNumbers(lexeme, true);
          type = lexeme == "." ? TokenType.REGULAR : TokenType.REAL;
          break;
        // numbers
        case >= '0' and <= '9':
        {
          bool point;
          lexeme = lexeme.ConcatByte(currentByte);
          (lexeme, point) = ConsumeNumbers(lexeme, false);
          type = point ? TokenType.REAL : TokenType.INTEGER;
          break;
        }
        case '[':
          lexeme = lexeme.ConcatByte(currentByte);
          type = TokenType.ARRAY_OPEN;
          break;
        case ']':
          lexeme = lexeme.ConcatByte(currentByte);
          type = TokenType.ARRAY_CLOSE;
          break;
        // keywords and regular
        default:
          (lexeme, type) = ConsumeKeyword(currentByte);
          break;
      }

      CurrentToken = new Token(type, lexeme, errorMessage);
      return CurrentToken;
    }

    public Token GetStreamContentToken()
    {
      var lexeme = new StringBuilder();

      while (!IsNextString("\r\nendstream") && !IsNextString("\rendstream") && !IsNextString("\nendstream"))
      {
        var val = _inputStream.ReadByte();
        if (val == -1) break;
        lexeme.Append(Encoding.UTF8.GetString(new [] { (byte)val }));
      }

      CurrentToken = new Token(TokenType.STREAM_CONTENT, lexeme.ToString());
      return CurrentToken;
    }

    public void ConsumeToken()
    {
      if (CurrentToken == null)
        GetToken();
      CurrentToken = null;
    }
    
    public void TryConsumeToken(List<Token> options)
    {
      var token = GetToken();
      if (!options.Any(x => x.Type == token.Type && x.Lexeme == token.Lexeme))
        throw new PdfParseException("Unexpected token :(   make this message a little better");
      ConsumeToken();
    }

    public void TryConsumeToken(TokenType type)
    {
      var token = GetToken();
      if (token.Type != type)
        throw new PdfParseException($"Unexpected token :(    Expected {type} but found '{token.Lexeme}'({token.Type})");
      ConsumeToken();
    }

    private bool IsNextString(string next)
    {
      var position = _inputStream.Position;
      var result = true;
      foreach (var c in next)
      {
        var nextByte = _inputStream.ReadByte();
        if (c == nextByte) continue;
        result = false;
        break;
      }
      _inputStream.Seek(position, SeekOrigin.Begin);
      return result;
    }

    private (string, TokenType) ConsumeKeyword(int currentByte)
    {
      var type = TokenType.REGULAR;
      var lexeme = "".ConcatByte(currentByte);

      foreach (var key in KeywordsOperators.Values.Keys.Where(x => x.StartsWith(lexeme)))
      {
        var leftover = key[1..];
        if (!IsNextString(leftover)) continue;
        lexeme += leftover;
        type = KeywordsOperators.Values[key];
        _inputStream.Seek(leftover.Length, SeekOrigin.Current);
        break;
      }

      return (lexeme, type);
    }

    private (string, bool) ConsumeNumbers(string lexeme, bool point)
    {
      var nextByte = _inputStream.ReadByte();
      while (nextByte != -1)
      {
        if (nextByte is >= '0' and <= '9')
        {
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else if (nextByte is '.' && !point)
        {
          lexeme = lexeme.ConcatByte(nextByte);
          point = true;
        }
        else
        {
          _inputStream.Seek(-1, SeekOrigin.Current);
          break;
        }
        nextByte = _inputStream.ReadByte();
      }
      return (lexeme, point);
    }
    
    private string ConsumeWhile(string lexeme, Func<int, bool> pred)
    {
      var nextByte = _inputStream.ReadByte();
      while (nextByte is not -1)
      {
        if (pred(nextByte))
        {
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else
        {
          _inputStream.Seek(-1, SeekOrigin.Current);
          break;
        }
        nextByte = _inputStream.ReadByte();
      }
      return lexeme;
    }

    private string ConsumeUntil(string lexeme, Func<int, bool> pred)
    {
      return ConsumeWhile(lexeme, (int utfByte) => !pred(utfByte));
    }
    
    private static bool IsRegular(int character)
    {
      return !IsDelimiter(character) && !IsWhitespace(character);
    }
    private static bool IsDelimiter(int character)
    {
      return character is 40 or 41 or 60 or 62 or 91 or 93 or 123 or 125 or 47 or 37;
    }
    private static bool IsWhitespace(int character)
    {
      return character is 0 or 9 or 10 or 12 or 13 or 32;
    }
    
    private static bool IsHex(char c) => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';

    public void Dispose()
    {
      _inputStream.Close();
      GC.SuppressFinalize(this);
    }
  }
}