using System;
using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString("(oh yeah \\073^\\51)");
      var token = lexer.GetNextToken();
      while (token.Type != TokenType.EOF)
      {
        Console.WriteLine($"{token.Type} - '{token.Lexeme}'");
        token = lexer.GetNextToken();
      }
    }
  }
}
