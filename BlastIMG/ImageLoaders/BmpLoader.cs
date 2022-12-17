using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlastIMG.ImageLoaders;

enum CompressionMethod
{
    BI_RGB,            // 0 
    BI_RLE8,           // 1
    BI_RLE4,           // 2
    BI_BITFIELDS,      // 3
    BI_JPEG,           // 4
    BI_PNG,            // 5
    BI_ALPHABITFIELDS, // 6
    BI_CMYK,           // 11
    BI_CMYKRLE8,       // 12
    BI_CMYKRLE4,       // 13
}

class BitmapFileHeader
{
    public string MagicNumber { get; set; }
    public int FileSize { get; set; }
    // there are 4 bytes that are set by the program that creates the image but we probably don't need it
    public int PixelArrayOffset { get; set; }
}

class BitmapInfoHeader
{ 
    public int HeaderSize { get; set; }
    public int BitmapWidth { get; set; }
    public int BitmapHeight { get; set; }
    public short ColorPlanes { get; set; } // must be 1
    public short BitsPerPixel { get; set; }
    public CompressionMethod CompressionMethod { get; set; }
    public int RawImageSize { get; set; } // can be set to 0
    public int HorizontalResolution { get; set; } // ppm
    public int VerticalResolution { get; set; } // ppm 
    public int ColorPalette { get; set; } // 0 for all
    public int ImportantColors { get; set; }
}

public class BmpLoader : IImageLoader
{
    public static Image Load(string filepath)
    {
        using var imageFile = File.OpenRead(filepath);

        var fileHeader = imageFile.ReadBytes(0, 14);
        var parsedFileHeader = new BitmapFileHeader
        {
            MagicNumber = Encoding.Default.GetString(fileHeader[..2]),
            FileSize = BitConverter.ToInt32(fileHeader[2..6]),
            PixelArrayOffset = BitConverter.ToInt32(fileHeader[10..14])
        };

        var headerSize = BitConverter.ToInt32(imageFile.ReadBytes(14, 4));
        var dibHeader = imageFile.ReadBytes(14, headerSize);
        // TODO pull more of the header data here if the header isn't the default header
        var parsedDibHeader = new BitmapInfoHeader
        {
            HeaderSize = headerSize,
            BitmapWidth = BitConverter.ToInt32(dibHeader[4..8]),
            BitmapHeight = BitConverter.ToInt32(dibHeader[8..12]),
            ColorPlanes = BitConverter.ToInt16(dibHeader[12..14]),
            BitsPerPixel = BitConverter.ToInt16(dibHeader[14..16]),
            CompressionMethod = BitConverter.ToInt32(dibHeader[16..20]).GetCompressionMethod(),
            RawImageSize = BitConverter.ToInt32(dibHeader[20..24]),
            HorizontalResolution = BitConverter.ToInt32(dibHeader[24..28]),
            VerticalResolution = BitConverter.ToInt32(dibHeader[28..32]),
            ColorPalette = BitConverter.ToInt32(dibHeader[32..36]),
            ImportantColors = BitConverter.ToInt32(dibHeader[36..40]),
        };
         
        Console.Out.WriteLine($"{parsedFileHeader.MagicNumber}");
        Console.Out.WriteLine($"{parsedFileHeader.FileSize}");
        Console.Out.WriteLine($"{parsedFileHeader.PixelArrayOffset}");
        Console.Out.WriteLine($"Width: {parsedDibHeader.BitmapWidth}");
        Console.Out.WriteLine($"Height: {parsedDibHeader.BitmapHeight}");
        Console.Out.WriteLine(JsonConvert.SerializeObject(parsedDibHeader, Formatting.Indented, new JsonConverter[] {new StringEnumConverter()}));
        
        return new Image(); // DUMMY FOR NOW
    }
}

static class BmpLoaderExtensions {
    public static CompressionMethod GetCompressionMethod(this int method)
    {
        return method switch
        {
            0 => CompressionMethod.BI_RGB,
            1 => CompressionMethod.BI_RLE8,
            2 => CompressionMethod.BI_RLE4,
            3 => CompressionMethod.BI_BITFIELDS,
            4 => CompressionMethod.BI_JPEG,
            5 => CompressionMethod.BI_PNG,
            6 => CompressionMethod.BI_ALPHABITFIELDS,
            11 => CompressionMethod.BI_CMYK,
            12 => CompressionMethod.BI_CMYKRLE8,
            13 => CompressionMethod.BI_CMYKRLE4,
            _ => throw new ArgumentException("Unknown Bitmap Compression Method", nameof(method))
        };
    }
}