using BlastPDF.Builder.Interfaces;

namespace BlastPDF.Builder.Graphics;

public class PdfGraphicsGroup : PdfGraphicsObject, IPdfStreamExporter {
  public static PdfGraphicsGroup Create() {
    return new PdfGraphicsGroup();
  }

  public PdfGraphicsGroup Add(PdfGraphicsObject graphicsObject) {
    SubObjects.Add(graphicsObject);
    return this;
  }
}