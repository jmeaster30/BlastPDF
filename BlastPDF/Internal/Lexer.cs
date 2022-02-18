using System;
using System.IO;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal
{
  public class Lexer : IDisposable
  {
    private readonly Stream _inputStream;

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

    public Token GetNextToken()
    {
      var currentByte = _inputStream.ReadByte();

      if (currentByte < 0) return new(TokenType.EOF, "");

      TokenType type;
      var lexeme = "";
      
      switch (currentByte)
      {
        case '(' or ')' or '<' or '>' or '[' or ']' or '{' or '}' or '/' or '%':
          type = TokenType.DELIMITER;
          lexeme = BPH.ByteNumToString(currentByte);
          break;
        case 0 or 9 or 10 or 12 or 13 or 32:
          type = TokenType.WHITESPACE;
          lexeme = BPH.ByteNumToString(currentByte);
          break;
        default:
          lexeme = BPH.ByteNumToString(currentByte); 
          type = TokenType.REGULAR;
          break;
      }

      return new Token(type, lexeme);
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