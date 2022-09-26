using BlastIMG.ImageLoaders;

namespace BlastIMG;

public enum FileFormat
{
    BMP,
    PNG,
    JPEG,
    QOI
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
            _ => throw new NotImplementedException("Format not currently supported :(")
        };
}