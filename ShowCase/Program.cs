using System;
using System.Linq;
using BlastPDF.Internal;
using BlastPDF.Internal.Structure;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString("(wow \\102\\117\\117\\102\\111\\105\\123)");
      var parser = new Parser(lexer);
      var literal = parser.ParseLiteralString();
      
      Console.WriteLine($"{literal.ObjectType} - '{literal.StringValue}' - '{literal.ResolvedValue}'");
    }
  }
}
