using BlastPDF.Internal;
using BlastPDF.Internal.Structure;
using System.Collections.Generic;

namespace BlastPDF.Loader
{
  public class PDFLoader
  {
    public static IEnumerable<PdfObject> LoadFromString(string source)
    {
      var lexer = Lexer.FromString(source);
      var parser = new Parser(lexer);
      return parser.Parse();
    }

    public static IEnumerable<PdfObject> LoadFromFile(string filename)
    {
      var lexer = Lexer.FromFile(filename);
      var parser = new Parser(lexer);
      return parser.Parse();
    }
  }
}