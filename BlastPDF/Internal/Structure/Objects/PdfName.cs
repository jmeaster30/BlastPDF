using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Exceptions;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfName : PdfObject
{
    public Token Token { get; set; }
    public string Name { get; set; }
    
    public PdfName(Token token) : base(PdfNodeType.NAME)
    {
        Token = token;
        Name = ResolveName(Token.Lexeme);
    }

    public override void Print()
    {
        Console.Write(Token.Lexeme);
    }

    private static string ResolveName(string lexeme)
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