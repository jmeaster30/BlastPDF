
using System;
using System.Collections;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal
{
  public enum TokenType
  {
    ERROR, REGULAR, EOF, EOL, WHITESPACE,
    COMMENT, BOOLEAN, INTEGER, REAL,
    LITERAL, HEX, NAME, NULL,
    ARRAY_OPEN, ARRAY_CLOSE,
    DICT_OPEN, DICT_CLOSE, KEYWORD
  }

  public class Token
  {
    public TokenType Type { get; private set; }
    public string Lexeme { get; private set; }
    public string ResolvedValue { get; private set; }
    public string ErrorMessage { get; private set; }

    private Token()
    {
    }

    public Token(TokenType type, string lexeme, string errorMessage = "") {
      Type = type;
      Lexeme = lexeme;
      ErrorMessage = errorMessage;
      ResolveTokenValue();
    }

    private void ResolveTokenValue()
    {
      ResolvedValue = Type switch
      {
        TokenType.NAME => ResolveName(Lexeme),
        TokenType.HEX => ResolveHexString(Lexeme),
        TokenType.LITERAL => ResolveLiteralString(Lexeme),
        _ => Lexeme,
      };
    }

    private string ResolveName(string lexeme)
    {
      var resolved = "";
      var idx = 1;
      while (idx < lexeme.Length)
      {
        var c = lexeme[idx];
        switch (c)
        {
          case '#' when idx >= lexeme.Length - 2 || !IsHex(lexeme[idx + 1]) || !IsHex(lexeme[idx + 2]):
            Type = TokenType.ERROR;
            ErrorMessage = "String ends with a # symbol. # should be followed by 2 hex digits.";
            idx += 2;
            break;
          case '#' when IsHex(lexeme[idx + 1]) && IsHex(lexeme[idx + 2]):
            resolved += HexToChar(lexeme[(idx + 1)..(idx + 3)]);
            idx += 2;
            break;
          default:
            resolved += c;
            break;
        }

        idx += 1;
      }

      return resolved;
    }

    private string ResolveHexString(string lexeme)
    {
      var resolved = lexeme[1..^1];
      if (!resolved.All(x => IsHex(x) || char.IsWhiteSpace(x)))
      {
        Type = TokenType.ERROR;
        ErrorMessage = "Hex string contains invalid characters.";
      }
      else
      {
        resolved = resolved.Filter(IsHex);
        if (resolved.Length % 2 == 1) resolved += '0';
        resolved = resolved.Window(2, HexToChar);
      }
      return resolved;
    }

    private static string ResolveLiteralString(string lexeme)
    {
      var resolved = new StringBuilder();
      for(var i = 1; i < lexeme.Length - 1; i++)
      {
        if (lexeme[i] == '\\' && i < lexeme.Length - 2)
        {
          switch (lexeme[i + 1])
          {
            case 'n': resolved.Append('\n');
              break;
            case 'r': resolved.Append('\r');
              break;
            case 't': resolved.Append('\t');
              break;
            case 'b': resolved.Append('\b');
              break;
            case 'f': resolved.Append('\f');
              break;
            case '(': resolved.Append('(');
              break;
            case ')': resolved.Append(')');
              break;
            case '\\': resolved.Append('\\');
              break;
            case '\r':
              if (i < lexeme.Length - 3 && lexeme[i + 2] == '\n') i += 1;
              break;
            case '\n':
              break;
            default: resolved.Append(lexeme[i + 1]);
              break;
          }
          i += 1;
        }
        else
        {
          resolved.Append(lexeme[i]);
        }
      }
      return resolved.ToString();
    }

    private static bool IsHex(char c) => c is >= '0' and <= '9' or >= 'A' and <= 'F' or >= 'a' and <= 'f';

    private static string HexToChar(string hex)
    {
      var top = hex[0] switch {
        >= '0' and <= '9' => hex[0] - 48,
        >= 'A' and <= 'F' => hex[0] - 65 + 10,
        >= 'a' and <= 'f' => hex[0] - 97 + 10,
        _ => 0
      };
      var bottom = hex[1] switch {
        >= '0' and <= '9' => hex[1] - 48,
        >= 'A' and <= 'F' => hex[1] - 65 + 10,
        >= 'a' and <= 'f' => hex[1] - 97 + 10,
        _ => 0
      };
      return $"{Convert.ToChar((byte)(top * 16 + bottom))}";
    }
  }
}