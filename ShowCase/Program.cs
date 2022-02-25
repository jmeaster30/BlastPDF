using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString(@"
<<
  /Length 5
  /Filter /FlateDecode
>>
stream
oh fuck yeah baby steak monday
endstream
");
      var parser = new Parser(lexer);
      var objs = parser.Parse();
      
      foreach (var node in objs)
      {
        node.Print();
      }
    }
  }
}
