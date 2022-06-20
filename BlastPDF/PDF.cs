using BlastPDF.Internal;
using BlastPDF.Internal.Structure;
using BlastPDF.Internal.Structure.File;
using System.Collections.Generic;

namespace BlastPDF
{
  public class PDF
  {
    private PdfInternalFile file { get; set; }
    private IEnumerable<PdfObject> objects { get; set; }

    private PDF() {}

    public static PDF LoadFromFile(string filename) {
      return null;
    }

    public static PDF LoadFromString(string source) {
      var lexer = Lexer.FromString(source);
      var parser = new Parser(lexer);
      return new PDF() { objects = parser.Parse() };
    }

    public void printAllObjects() {
      foreach (var node in objects) {
        node.Print();
      }
    }
  }
}