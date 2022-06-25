using BlastPDF.Internal;
using BlastPDF.Internal.Structure.File;
using System.Collections.Generic;

namespace BlastPDF.Loader
{
  public class PDFLoader
  {
    public static PdfInternalFile LoadFromString(string source)
    {
      var lexer = Lexer.FromString(source);
      var parser = new Parser(lexer);
      return parser.ParseFile();
    }

    public static PdfInternalFile LoadFromFile(string filename)
    {
      var lexer = Lexer.FromFile(filename);
      var parser = new Parser(lexer);
      return parser.ParseFile();
    }
  }
}