using System;
using System.Linq;
using BlastPDF.Internal.Exceptions;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfHexString : PdfObject
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public PdfHexString(Token value) : base(PdfObjectType.HEX_STRING)
    {
        Token = value;
        Value = ResolveHexString(Token.Lexeme);
    }

    public override void Print()
    {
        Console.Write(Token.Lexeme);
    }
    
    private static string ResolveHexString(string lexeme)
    {
        var resolved = lexeme[1..^1];
        if (!resolved.All(x => IsHex(x) || char.IsWhiteSpace(x)))
            throw new PdfParseException($"This hex string <{lexeme}> contains invalid characters. Allowed characters are whitespace, 0-9, a-f, and A-F");

        resolved = resolved.Filter(IsHex); 
        if (resolved.Length % 2 == 1) resolved += '0';
        resolved = resolved.Window(2, HexToChar);
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