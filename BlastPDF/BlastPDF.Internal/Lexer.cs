using System;
using System.IO;
using System.Text;
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
      var currentPosition = InputStream.Position;
      var currentByte = InputStream.ReadByte();

      if (currentByte < 0)
        return new Token(TokenType.EOF, "");

      TokenType type = TokenType.ERROR;
      var lexeme = "";

      switch (currentByte)
      {
        case 0:  // Null
        case 9:  // Horizontal Tab
        case 12: // Form Feed
        case 32: // Space
          // FIXME : PDF 32000-1:2008 Section 7.2.2 
          // FIXME : ...PDF treats any sequence of consecutive white-space characters as one character.
          type = TokenType.WHITESPACE;
          lexeme = lexeme.ConcatByte(currentByte);
          break;
        case 10: // Line Feed
          type = TokenType.EOL;
          lexeme = lexeme.ConcatByte(currentByte);
          break;
        case 13: // Carriage Return
          // PDF 32000-1:2008 Section 7.2.2 
          // The combination of a CARRIAGE RETURN followed immediately by a LINE FEED shall be treated as one EOL marker.
          type = TokenType.EOL;
          lexeme = lexeme.ConcatByte(currentByte);
          var nextByte = InputStream.ReadByte();
          if (nextByte != -1 && nextByte == 10) {
            lexeme = lexeme.ConcatByte(nextByte);
          } else {
            InputStream.Seek(-1, SeekOrigin.Current);
          }
          break;
      }

      return new Token(type, lexeme);
    }

    public void Dispose()
    {
      InputStream.Close();
      GC.SuppressFinalize(this);
    }
  }
}