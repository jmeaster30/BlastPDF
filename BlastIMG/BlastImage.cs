namespace BlastIMG;

public enum FileFormat
{
    BMP,
    PNG,
    JPEG,
    QOI
}

public class Pixel
{
    public int X { get; }
    public int Y { get; }
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }
}

public class Image
{
    public FileFormat Format { get; }
    public int Width { get; }
    public int Height { get; }
    public Pixel[,] Pixels { get; }

    public Pixel GetPixel(int x, int y)
    {
        return Pixels[x, y];
    }

    public static Image Load(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentException("Image path cannot be null or whitespace.", nameof(imagePath));
        
        
        return new Image();
    }
}