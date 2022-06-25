using System;
using BlastPDF.Loader;
using BlastPDF.Internal.Structure;
using BlastPDF.Internal.Structure.File;
using System.Collections.Generic;

namespace BlastPDF
{
  public class PDF
  {
    private PdfInternalFile file { get; set; }

    private PDF() {}

    public static PDF LoadFromFile(string filename) {
      return new PDF(){ file = PDFLoader.LoadFromFile(filename) };
    }

    public static PDF LoadFromString(string source) {
      return new PDF(){ file = PDFLoader.LoadFromString(source) };
    }

    public void printAllObjects() {
      file.Header.Print();
      foreach (var bodySection in file.Body) {
        bodySection.Contents.ForEach(x => x.Print());
        Console.WriteLine(">>>>>>");
      }
    }
  }
}