using System;
using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString("<42 4f 4f 4253><3A29><404>");
      var token = lexer.GetNextToken();
      while (token.Type != TokenType.EOF)
      {
        Console.WriteLine($"{token.Type} - '{token.Lexeme}' - '{token.ResolvedValue}'");
        token = lexer.GetNextToken();
      }
    }
  }
}
