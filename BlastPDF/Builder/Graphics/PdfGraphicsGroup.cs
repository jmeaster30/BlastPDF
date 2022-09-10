namespace BlastPDF.Builder.Graphics;

public class PdfGraphicsGroup : PdfGraphicsObject {
  public static new PdfGraphicsGroup Create() {
    return new PdfGraphicsGroup();
  }

  public PdfGraphicsGroup Add(PdfGraphicsObject graphicsObject) {
    SubObjects.Add(graphicsObject);
    return this;
  }
}