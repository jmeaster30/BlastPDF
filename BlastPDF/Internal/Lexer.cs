using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

      CurrentToken = new Token(type, lexeme);
      return CurrentToken;
    }

    public void ConsumeToken()
    {
      CurrentToken = null;
    }

    public IEnumerable<Token> TryGetTokens(string match)
    {
      var found = new List<Token>();

      foreach (var c in match)
      {
        var token = GetToken();
        if (token.Lexeme == Char.ToString(c)) {
          found.Add(token);
        } else {
          break;
        }
        ConsumeToken();
      }

      if (found.Count == match.Length) return found;
      
      _inputStream.Seek(-1 * (found.Count + 1), SeekOrigin.Current);
      CurrentToken = null;
      GetToken();
      found.Clear();

      return found;
    }

    public void Dispose()
    {
      _inputStream.Close();
      GC.SuppressFinalize(this);
    }
  }
}