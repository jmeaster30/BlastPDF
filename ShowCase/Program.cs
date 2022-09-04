using BlastPDF;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using System;
using System.IO;

namespace ShowCase;

public class Program {
  public static void Main(string[] args) {
    //Console.WriteLine("STARTING....");
    //var pdf = PDF.LoadFromFile("../Samples/pdf-sample.pdf");

    //       var pdf = PDF.LoadFromString(@"
    // % PDF 1.7
    // 1 0 obj
    //   << /wow 5 /test 234 /fart [1 2 3]>>
    // stream
    //   oh fuck yean baby steak monday
    // endstream
    // endobj
    // ");

    //pdf.printAllObjects();

    string path = "test.pdf";

    if (File.Exists(path)) {
      File.Delete(path);
    }

    using FileStream fs = File.Create(path);
    PdfDocument.Create()
      .AddPage(PdfPage.Create()
        .AddGraphics(
          PdfPath.Start()
            .Move(10, 10)
            .Line(600, 790)
            .Paint(PaintMode.Stroke)
            .LineWidth(10)))
      .Export(fs);
  }
}
