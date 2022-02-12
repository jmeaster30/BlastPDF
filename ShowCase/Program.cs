using System;
using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      Lexer lexer = Lexer.FromString("<BBAABBABAB>()");
      var token = lexer.GetNextToken();
      while (token.Type != TokenType.EOF)
      {
        Console.WriteLine($"{token.Type} - '{token.Lexeme}'");
        token = lexer.GetNextToken();
      }
    }
  }
}
