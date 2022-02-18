using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal;

public static class TokenExtensions
{
    private static string FindOctal(string section) => section[..section.IndexOf(x => x is not (< '8' and >= '0'))];

    public static string ResolveLiteralString(this string lexeme)
    {
        var resolved = new StringBuilder();
        for (var i = 1; i < lexeme.Length - 1; i++)
        {
            if (lexeme[i] == '\\' && i < lexeme.Length - 2)
            {
                switch (lexeme[i + 1])
                {
                    case 'n':
                        resolved.Append('\n');
                        break;
                    case 'r':
                        resolved.Append('\r');
                        break;
                    case 't':
                        resolved.Append('\t');
                        break;
                    case 'b':
                        resolved.Append('\b');
                        break;
                    case 'f':
                        resolved.Append('\f');
                        break;
                    case '(':
                        resolved.Append('(');
                        break;
                    case ')':
                        resolved.Append(')');
                        break;
                    case '\\':
                        resolved.Append('\\');
                        break;
                    case '\r':
                        if (i < lexeme.Length - 3 && lexeme[i + 2] == '\n') i += 1;
                        break;
                    case '\n':
                        break;
                    case >= '0' and < '8':
                        var octal = FindOctal(lexeme[(i + 1)..Math.Min(lexeme.Length, i + 4)]);
                        resolved.Append(
                            Encoding.UTF8.GetString(new[] {(byte) Convert.ToInt32(octal.PadLeft(3, '0'), 8)}));
                        i += octal.Length - 1;
                        break;
                    default:
                        resolved.Append(lexeme[i + 1]);
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
    
    public static string ResolveHexString(this string lexeme)
    {
        var resolved = lexeme[1..^1];
        if (!resolved.All(x => IsHex(x) || char.IsWhiteSpace(x)))
        {
            throw new PdfParseException($"This hex string <{lexeme}> contains invalid characters. Allowed characters are whitespace, 0-9, a-f, and A-F");
        }
        else
        {
            resolved = resolved.Filter(IsHex);
            if (resolved.Length % 2 == 1) resolved += '0';
            resolved = resolved.Window(2, HexToChar);
        }
        return resolved;
    }

    public static string ResolveName(this string lexeme)
    {
        var resolved = "";
        var idx = 1;
        while (idx < lexeme.Length)
        {
            var c = lexeme[idx];
            switch (c)
            {
                case '#' when idx >= lexeme.Length - 2 || !IsHex(lexeme[idx + 1]) || !IsHex(lexeme[idx + 2]):
                    throw new PdfParseException($"String '{lexeme}' ends with a # symbol. # should be followed by 2 hex digits.");
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