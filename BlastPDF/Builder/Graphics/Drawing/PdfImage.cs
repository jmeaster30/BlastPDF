using System;
using BlastIMG;
using BlastIMG.ImageLoaders;

namespace BlastPDF.Builder.Graphics.Drawing;

public class PdfImage : PdfObject {

  public int Width { get; set; }
  public int Height { get; set; }
  public PdfColorSpace ColorSpace { get; set; }
  public int BitsPerComponent { get; set; }

  // TODO add the other options from table 89

  public static PdfImage FromURL(string url, PdfColorSpace colorSpace) {
    throw new NotImplementedException("I need to think more about this one :(");
  }

  public static PdfImage FromFile(string filename, FileFormat format, PdfColorSpace colorSpace)
  {
    var image = Image.Load(filename, format);
    var result = new PdfEmbeddedImage {
      Width = (int)image.Width,
      Height = (int)image.Height,
      ColorSpace = colorSpace,
      BitsPerComponent = 8, // TODO I would like this to be figured out from the pixel format in the image
    };

    var pixelSize = colorSpace switch {
      PdfColorSpace.DeviceRGB => 3,
      PdfColorSpace.DeviceCMYK => 4,
      PdfColorSpace.DeviceGray => 1,
      _ => throw new NotImplementedException("We don't support these color spaces for images sorry :("),
    };
    var dataSize = image.Width * image.Height * pixelSize;

    var data = new byte[dataSize];
    // row first order for the data stream
    for (uint y = 0; y < image.Height; y++) {
      for (uint x = 0; x < image.Width; x++) {
        var startIndex = (x + y * image.Width) * pixelSize;
        var color = image.GetPixel(x, y);
        switch (colorSpace) {
          case PdfColorSpace.DeviceRGB: {
            data[startIndex] = color.R;
            data[startIndex + 1] = color.G;
            data[startIndex + 2] = color.B;
            break;
          }
          case PdfColorSpace.DeviceGray: {
            data[startIndex] = ToGray(color.R, color.G, color.B);
            break;
          }
          case PdfColorSpace.DeviceCMYK: {
            var (cc, cm, cy, ck) = ToCmyk(color.R, color.G, color.B);
            data[startIndex] = cc;
            data[startIndex + 1] = cm;
            data[startIndex + 2] = cy;
            data[startIndex + 3] = ck;
            break;
          }
        }
      }
    }
    result.ImageData = data;

    return result;
  }

  private static (byte, byte, byte, byte) ToCmyk(byte r, byte g, byte b) {
    var rPrime = r / 255.0;
    var gPrime = g / 255.0;
    var bPrime = b / 255.0;
    var k = 1 - Math.Max(rPrime, Math.Max(gPrime, bPrime));
    var c = (1 - rPrime - k) / (1 - k);
    var m = (1 - gPrime - k) / (1 - k);
    var y = (1 - bPrime - k) / (1 - k);
    return ((byte)(c * 255), (byte)(m * 255), (byte)(y * 255), (byte)(k * 255));
  }

  private static byte ToGray(byte r, byte g, byte b) {
    return (byte)(0.299 * r + 0.587 * g + 0.114 * b);
  }
}

public enum ImageFormat
{
  BMP,
  QOI,
  PNG,
  JPEG,
  GIF
}

public class PdfEmbeddedImage : PdfImage {
  public byte[] ImageData { get; set; }
  
}

public class PdfExternalImage : PdfImage {
  public string URL { get; set; }
}



public static class PdfImageExtensions
{
  public static PdfGraphicsObject InlineImage(ImageFormat imageFormat, string filename)
  {
    return null;
  }
}

