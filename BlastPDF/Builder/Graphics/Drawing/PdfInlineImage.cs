using BlastPDF.Filter;
using SharperImage;
using SharperImage.Formats;

namespace BlastPDF.Builder.Graphics.Drawing;

public class PdfInlineImage : PdfGraphicsObject
{

  public uint Width { get; set; }
  public uint Height { get; set; }
  public PdfColorSpace ColorSpace { get; set; }
  public int BitsPerComponent { get; set; }
  public PdfFilter[] Filters { get; set; } // SHOULD THIS ACTUALLY BE ON THE PdfGraphicsObject?

  // TODO add the other options from table 89

  public IImage ImageData { get; set; }

  public static PdfInlineImage FromFile(string filename, FileFormat format, PdfColorSpace colorSpace, PdfFilter[] filters)
  {
    var image = IImage.Decode(filename, format);
    return FromImage(image, colorSpace, filters);
  }
  
  public static PdfInlineImage FromImage(IImage image, PdfColorSpace colorSpace, PdfFilter[] filters)
  {
    var result = new PdfInlineImage {
      Width = image.Width(),
      Height = image.Height(),
      ColorSpace = colorSpace,
      BitsPerComponent = 8, // TODO I would like this to be figured out from the pixel format in the image
      ImageData = image,
      Filters = filters,
    };

    if (filters is null)
    {
      result.Filters = new[] {PdfFilter.AsciiHex};
    }

    return result;
  }
}

public static class PdfInlineImageExtensions
{
  public static PdfGraphicsObject InlineImage(this PdfGraphicsObject graphics, string filename, FileFormat format, PdfColorSpace colorSpace = PdfColorSpace.DeviceRGB, PdfFilter[] filters = null)
  {
    graphics.SubObjects.Add(PdfInlineImage.FromFile(filename, format, colorSpace, filters));
    return graphics;
  }
}

