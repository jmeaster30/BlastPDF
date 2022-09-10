using BlastPDF.Exporter.Basic;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using System;
using System.IO;

namespace ShowCase;

public class Program {
  public static void Main(string[] args) {
    string path = "test.pdf";

    if (File.Exists(path)) {
      File.Delete(path);
    }

    var graphicsGroup = PdfGraphicsGroup.Create();

    var triangleWidth = 5.0M;
    for(int i = 0; i < 96; i++) {
      graphicsGroup.Add(
        DrawTriangle(0, 0, triangleWidth)
          .Translate(500, 500)
          .Rotate(i * (decimal)Math.PI / 96)
      );
      triangleWidth *= 1.1M;
    }

    using FileStream fs = File.Create(path);
    PdfDocument.Create()
      .AddPage(PdfPage.Create()
        .DotsPerInch(100)
        .Width(10)
        .Height(10)
        .AddGraphics(graphicsGroup.LineWidth(2)))
      .Save(fs);
  }

  public static PdfGraphicsObject DrawTriangle(decimal x, decimal y, decimal length)
  {
    var radius = length / (2 * (decimal)Math.Cos(Math.PI / 6));
    var apothem = length * (decimal)Math.Tan(Math.PI / 6) / 2;

    var top = (x, y + radius);
    var left = (x - (length / 2), y - apothem);
    var right = (x + (length / 2), y - apothem);

    return PdfPath.Start()
      .Move(top.Item1, top.Item2)
      .Line(left.Item1, left.Item2)
      .Line(right.Item1, right.Item2)
      .Paint(PaintModeEnum.CloseStroke);
  }
}
