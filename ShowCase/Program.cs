using BlastPDF.Internal;

namespace ShowCase
{
  class Program
  {
    static void Main(string[] args)
    {
      var lexer = Lexer.FromString(@"
[ <b00b> 
  <<
/TEST (value)
/Length <<
  /Butt /Stuff
>>
>> <dead> ]
");
      var parser = new Parser(lexer);
      var literal = parser.ParseObject();
      
      literal.Print();
    }
  }
}
