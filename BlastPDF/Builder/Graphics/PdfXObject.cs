
namespace BlastPDF.Builder.Graphics;

public class PdfXObject : PdfGraphicsObject {
  public string Resource { get; set; }
}

public static class PdfXObjectExtensions {
  public static PdfGraphicsObject Resource(this PdfGraphicsObject graphics, string resource) {
    graphics.SubObjects.Add(new PdfXObject { Resource = resource });
    return graphics;
  }
}