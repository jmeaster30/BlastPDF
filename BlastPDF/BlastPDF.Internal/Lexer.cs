using System;
using System.IO;
using System.Text;
using System.Threading.Tasks.Dataflow;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal
{
  public class Lexer : IDisposable
  {
    private readonly Stream InputStream;

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
      InputStream = file;
    }

    private Lexer(MemoryStream source)
    {
      InputStream = source;
    }

    public Token GetNextToken()
    {
      //var currentPosition = InputStream.Position;
      var currentByte = InputStream.ReadByte();

      if (currentByte < 0) return new Token(TokenType.EOF, "");

      TokenType type = TokenType.REGULAR;
      var lexeme = "";

      if (currentByte is 0 or 9 or 12 or 32)
      {
        // PDF 32000-1:2008 Section 7.2.2 
        // PDF treats any sequence of consecutive white-space characters as one character.
        type = TokenType.WHITESPACE;
        lexeme = lexeme.ConcatByte(currentByte);
        lexeme = ConsumeWhile(lexeme, (int utfbyte) => {
          return utfbyte is 0 or 9 or 12 or 32;
        });
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
       
        var nextByte = InputStream.ReadByte();
        if (nextByte is 10)
        {
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else if (nextByte is not -1)
        {
          InputStream.Seek(-1, SeekOrigin.Current);
        }
      }
      else if (currentByte is '%')
      {
        // PDF 32000-1:2008 Section 7.2.3
        // The comment consists of all characters after the PERCENT SIGN and up to but not including the end of the line, including
        // regular, delimiter, SPACE (20h), and HORZONTAL TAB characters (09h).
        type = TokenType.COMMENT;
        lexeme = lexeme.ConcatByte(currentByte);
        lexeme = ConsumeUntil(lexeme, (int utfbyte) => {
          return utfbyte is 10 or 13;
        });
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
        lexeme = ConsumeWhile(lexeme, (int utfbyte) => {
          return (utfbyte is >= '!' and <= '~') && IsRegular(utfbyte);
        });
      }
      else // keywords and regular
      {

      }

      return new Token(type, lexeme);
    }

    private string ConsumeWhile(string lexeme, Func<int, bool> pred)
    {
      var nextByte = InputStream.ReadByte();
      while (nextByte is not -1)
      {
        if (pred(nextByte))
        {
          lexeme = lexeme.ConcatByte(nextByte);
        }
        else
        {
          InputStream.Seek(-1, SeekOrigin.Current);
          break;
        }
        nextByte = InputStream.ReadByte();
      }
      return lexeme;
    }

    private string ConsumeUntil(string lexeme, Func<int, bool> pred)
    {
      return ConsumeWhile(lexeme, (int utfbyte) => { return !pred(utfbyte); });
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
      InputStream.Close();
      GC.SuppressFinalize(this);
    }
  }
}