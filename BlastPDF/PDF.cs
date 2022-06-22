using BlastPDF.Loader;
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
      return new PDF(){ objects = PDFLoader.LoadFromFile(filename) };
    }

    public static PDF LoadFromString(string source) {
      return new PDF(){ objects = PDFLoader.LoadFromString(source) };
    }

    public void printAllObjects() {
      foreach (var node in objects) {
        node.Print();
      }
    }
  }
}