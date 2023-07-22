namespace BlastPDF.Builder.Graphics.Drawing;

public static class PdfImageExtensions {
    public static PdfGraphicsObject Image(this PdfGraphicsObject graphics, string resource) {
        graphics.SubObjects.Add(new PdfXObject { Resource = resource });
        return graphics;
    }
}