using System;
using System.IO;
using System.Text;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal
{
  public class Lexer : IDisposable
  {
    private readonly Stream _inputStream;

    public static Lexer FromFile(string filepath)
    {
      return new(File.OpenRead(filepath));
    }

    public static Lexer FromString(string source)
    {
      return new(new MemoryStream(Encoding.UTF8.GetBytes(source)));
    }

    private Lexer(FileStream file)
    {
      _inputStream = file;
    }

    private Lexer(MemoryStream source)
    {
      _inputStream = source;
    }

    public Token GetNextToken()
    {
      //var currentPosition = InputStream.Position;
      var currentByte = _inputStream.ReadByte();

      if (currentByte < 0) return new(TokenType.EOF, "");

      var type = TokenType.REGULAR;
      var lexeme = "";

      if (currentByte is 0 or 9 or 12 or 32)
      {
        // PDF 32000-1:2008 Section 7.2.2 
        // PDF treats any sequence of consecutive white-space characters as one character.
        type = TokenType.WHITESPACE;
        lexeme = lexeme.ConcatByte(currentByte);
        lexeme = ConsumeWhile(lexeme, (int utfByte) => utfByte is 0 or 9 or 12 or 32);
      }
      else if (currentByte is 10) // Line Feed
      {
        type = TokenType.EOL;
        lexeme = lexeme.ConcatByte(currentByte);
      }
      else if (currentByte is 13) // Carriage Return
      {
        // PDF 32000-1:2008 Section 7.2.2 
        // The combination of a CARRIAGE RETURN followed immediately by a LINE FEED shall be treated as one EOL marker.
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
      }
      else if (currentByte is '%')
      {
        // PDF 32000-1:2008 Section 7.2.3
        // The comment consists of all characters after the PERCENT SIGN and up to but not including the end of the line, including
        // regular, delimiter, SPACE (20h), and HORZONTAL TAB characters (09h).
        type = TokenType.COMMENT;
        lexeme = lexeme.ConcatByte(currentByte);
        lexeme = ConsumeUntil(lexeme, (int utfByte) => utfByte is 10 or 13);
      }
      else if (currentByte is '/') // Name
      {
        /* PDF 32000-1:2008 Section 7.3.5
          When writing a name in a PDF file, a SOLIDUS (2Fh) (/) shall be used to introduce a name. The SOLIDUS is
          not part of the name but is a prefix indicating that what follows is a sequence of characters representing the
          name in the PDF file and shall follow these rules:
          a) A NUMBER SIGN (23h) (#) in a name shall be written by using its 2-digit hexadecimal code (23), preceded
             by the NUMBER SIGN.
          b) Any character in a name that is a regular character (other than NUMBER SIGN) shall be written as itself or
             by using its 2-digit hexadecimal code, preceded by the NUMBER SIGN.
          c) Any character that is not a regular character shall be written using its 2-digit hexadecimal code, preceded
             by the NUMBER SIGN only.
          The token SOLIDUS (a slash followed by no regular characters) introduces a unique valid name defined by the
          empty sequence of characters.
        */
        type = TokenType.NAME;
        lexeme = lexeme.ConcatByte(currentByte);
        lexeme = ConsumeWhile(lexeme, (int utfByte) => utfByte is >= '!' and <= '~' && IsRegular(utfByte));
      }
      else if (currentByte is '(') // string
      {
        /* PDF 32000-1:2008 Section 7.3.4.2
          A literal string shall be written as an arbitrary number of characters enclosed in parentheses. Any characters
          may appear in a string except unbalanced parentheses (LEFT PARENTHESIS (28h) and RIGHT
          PARENTHESIS (29h)) and the backslash (REVERSE SOLIDUS (5Ch)), which shall be treated specially as
          described in this sub-clause. Balanced pairs of parentheses within a string require no special treatment.
        */
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
            else if (depth == 0)
              break;
            else if (depth > 0)
              depth -= 1;
          }
          else if (nextByte is '(')
          {
            if (escape)
              escape = false;
            else
              depth += 1;
          }
          else if (nextByte is '\\')
          {
            escape = !escape;
          }
          else
          {
            escape = false;
          }
          nextByte = _inputStream.ReadByte();
        }
      }
      else if (currentByte == '<') // Hexadecimal Strings OR the start token of the dictionary
      {
        /* PDF 32000-1:2008 Section 7.3.4.3
          Strings may also be written in hexadecimal form, which is useful for including arbitrary binary data in a PDF file.
          A hexadecimal string shall be written as a sequence of hexadecimal digits (0–9 and either A–F or a–f) encoded
          as ASCII characters and enclosed within angle brackets (using LESS-THAN SIGN (3Ch) and GREATER-
          THAN SIGN (3Eh)).
        */
        lexeme = lexeme.ConcatByte(currentByte);
        var nextByte = _inputStream.ReadByte();
        if (nextByte is '<')
        {
          type = TokenType.DICTOPEN;
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else if (nextByte is '>')
        {
          type = TokenType.HEX;
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else
        {
          type = TokenType.HEX;
          lexeme = lexeme.ConcatByte(nextByte);
          lexeme = ConsumeUntil(lexeme, (int utfByte) => utfByte is '>');
          nextByte = _inputStream.ReadByte(); // consume the > symbol
          lexeme = lexeme.ConcatByte(nextByte);
        }
      }
      else if (currentByte == '>')
      {
        lexeme = lexeme.ConcatByte(currentByte);
        var nextByte = _inputStream.ReadByte();
        if (nextByte is '>')
        {
          type = TokenType.DICTCLOSE;
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else
        {
          // I don't think we can hit this actually 
        }
      }
      else if (currentByte is '+' or '-') // numbers
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
      }
      else if (currentByte is '.') // numbers
      {
        lexeme = lexeme.ConcatByte(currentByte);
        (lexeme, _) = ConsumeNumbers(lexeme, true);
        type = lexeme == "." ? TokenType.REGULAR : TokenType.REAL;
      }
      else if (currentByte is >= '0' and <= '9') // numbers
      {
        bool point;
        lexeme = lexeme.ConcatByte(currentByte);
        (lexeme, point) = ConsumeNumbers(lexeme, false);
        type = point ? TokenType.REAL : TokenType.INTEGER;
      }
      else // keywords and regular
      {

      }

      return new Token(type, lexeme);
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

    public void Dispose()
    {
      _inputStream.Close();
      GC.SuppressFinalize(this);
    }
  }
}