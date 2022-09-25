using System.Drawing;
using System;

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

  public static PdfImage FromFile(string filename, PdfColorSpace colorSpace) {
    using var myBitmap = new Bitmap(filename);
    var result = new PdfEmbeddedImage {
      Width = myBitmap.Width,
      Height = myBitmap.Height,
      ColorSpace = colorSpace,
      BitsPerComponent = 8, // TODO I would like this to be figured out from the pixel format in the image
    };

    var pixel_size = colorSpace switch {
      PdfColorSpace.DeviceRGB => 3,
      PdfColorSpace.DeviceCMYK => 4,
      PdfColorSpace.DeviceGray => 1,
      _ => throw new NotImplementedException("We don't support these color spaces for images sorry :("),
    };
    var data_size = myBitmap.Width * myBitmap.Height * pixel_size;

    var data = new byte[data_size];
    // row first order for the data stream
    for (int y = 0; y < myBitmap.Height; y++) {
      for (int x = 0; x < myBitmap.Width; x++) {
        var start_index = (x + y * myBitmap.Width) * pixel_size;
        var color = myBitmap.GetPixel(x, y);
        switch (colorSpace) {
          case PdfColorSpace.DeviceRGB: {
            data[start_index] = color.R;
            data[start_index + 1] = color.G;
            data[start_index + 2] = color.B;
            break;
          }
          case PdfColorSpace.DeviceGray: {
            data[start_index] = ToGray(color.R, color.G, color.B);
            break;
          }
          case PdfColorSpace.DeviceCMYK: {
            var (C, M, Y, K) = ToCMYK(color.R, color.G, color.B);
            data[start_index] = C;
            data[start_index + 1] = M;
            data[start_index + 2] = Y;
            data[start_index + 3] = K;
            break;
          }
          default:
            // we won't get here because we already threw an exception above
            break;
        }
      }
    }
    result.ImageData = data;

    return result;
  }

  private static (byte, byte, byte, byte) ToCMYK(byte R, byte G, byte B) {
    double r_prime = R / 255.0;
    double g_prime = G / 255.0;
    double b_prime = B / 255.0;
    double K = 1 - Math.Max(r_prime, Math.Max(g_prime, b_prime));
    double C = (1 - r_prime - K) / (1 - K);
    double M = (1 - g_prime - K) / (1 - K);
    double Y = (1 - b_prime - K) / (1 - K);
    return ((byte)(C * 255), (byte)(M * 255), (byte)(Y * 255), (byte)(K * 255));
  }

  private static byte ToGray(byte R, byte G, byte B) {
    return (byte)(0.299 * R + 0.587 * G + 0.114 * B);
  }
}

public class PdfEmbeddedImage : PdfImage {
  public byte[] ImageData { get; set; }

}

public class PdfExternalImage : PdfImage {
  public string URL { get; set; }
}

