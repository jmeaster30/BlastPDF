using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlastPDF.Internal.Helpers;

namespace BlastPDF.Internal.Structure;

public class PdfLiteralString : PdfObject
{
    public Token Token { get; set; }
    public string Value { get; set; }

    public PdfLiteralString(Token value) : base(PdfNodeType.LITERAL_STRING)
    {
        Token = value;
        Value = ResolveLiteralString(Token.Lexeme);
    }

    public override void Print()
    {
        Console.Write(Token.Lexeme);
    }
    
    private static string FindOctal(string section) => section[..section.IndexOf(x => x is not (< '8' and >= '0'))];

    private static string ResolveLiteralString(string lexeme)
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
}