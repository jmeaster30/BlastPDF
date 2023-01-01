using BlastIMG.ImageLoaders;

namespace BlastIMG;

public enum FileFormat
{
    BMP,
    PNG,
    JPEG,
    QOI,
    GIF
}

public enum ColorFormat
{
    RGB,
    RGBA,
    CMYK,
    GRAY,
}

public struct Pixel
{
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public byte R { get; set; } = 0;
    public byte G { get; set; } = 0;
    public byte B { get; set; } = 0;
    public byte A { get; set; } = 255;

    public Pixel(int x, int y, byte r, byte g, byte b, byte a)
    {
        X = x;
        Y = y;
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public byte[] Cmyk() {
        var rPrime = R / 255.0;
        var gPrime = G / 255.0;
        var bPrime = B / 255.0;
        var k = 1 - Math.Max(rPrime, Math.Max(gPrime, bPrime));
        var c = (1 - rPrime - k) / (1 - k);
        var m = (1 - gPrime - k) / (1 - k);
        var y = (1 - bPrime - k) / (1 - k);
        return new[]{(byte)(c * 255), (byte)(m * 255), (byte)(y * 255), (byte)(k * 255)};
    }

    public byte Gray() {
        return (byte)(0.299 * R + 0.587 * G + 0.114 * B);
    }
}

public class Image
{
    public FileFormat Format { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public Pixel[,] Pixels { get; set; }

    public Pixel GetPixel(uint x, uint y) => Pixels[x, y];

    public static Image Load(string imagePath, FileFormat format) =>
        format switch
        {
            FileFormat.BMP => BmpLoader.Load(imagePath),
            FileFormat.QOI => QoiLoader.Load(imagePath),
            FileFormat.GIF => GifLoader.Load(imagePath),
            _ => throw new NotImplementedException("Format not currently supported :(")
        };

    public byte[] GetColorArray(ColorFormat format)
    {
        var bytes = new List<byte>();
        for (uint y = 0; y < Height; y++) {
            for (uint x = 0; x < Width; x++)
            {
                var c = GetPixel(x, y);
                bytes.AddRange(format switch
                {
                    ColorFormat.RGB => new[] {c.R, c.G, c.B},
                    ColorFormat.RGBA => new[] {c.R, c.G, c.B, c.A},
                    ColorFormat.CMYK => c.Cmyk(),
                    ColorFormat.GRAY => new[] {c.Gray()},
                    _ => Array.Empty<byte>()
                });
            }
        }
        return bytes.ToArray();
    }
}