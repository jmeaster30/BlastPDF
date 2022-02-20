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
      var lexer = Lexer.FromString("   [ 500 (oh yeah) /name <B00B1E> true null \n\n[    (test) 123 ] ] ");
      var parser = new Parser(lexer);
      var literal = parser.ParseObject();
      
      literal.Print();
    }
  }
}
