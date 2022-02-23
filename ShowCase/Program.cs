using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString("[ [ <b00b> /myname <dead> 1234 69] (yeah baby)    ");
      var parser = new Parser(lexer);
      var objs = parser.Parse();
      
      foreach (var node in objs)
      {
        node.Print();
      }
    }
  }
}
