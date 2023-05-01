using System.Text;
using BlastSharp.Lists;
using BlastSharp.Numbers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BlastIMG.ImageLoaders;

public enum CompressionMethod
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

public class BitmapFileHeader
{
    public string MagicNumber { get; set; }
    public int FileSize { get; set; }
    // there are 4 bytes that are set by the program that creates the image but we probably don't need it
    public int PixelArrayOffset { get; set; }
}

public class XYZTriple
{
    public int RedX { get; set; }
    public int RedY { get; set; }
    public int RedZ { get; set; }
    public int GreenX { get; set; }
    public int GreenY { get; set; }
    public int GreenZ { get; set; }
    public int BlueX { get; set; }
    public int BlueY { get; set; }
    public int BlueZ { get; set; }
}

public class BitmapInfoHeader
{
    public string HeaderType => HeaderSize switch
    {
        12 => "BITMAPCOREHEADER",
        64 => "OS22XBITMAPHEADER",
        16 => "OS22XBITMAPHEADER",
        40 => "BITMAPINFOHEADER",
        52 => "BITMAPV2INFOHEADER",
        56 => "BITMAPV3INFOHEADER",
        108 => "BITMAPV4HEADER",
        124 => "BITMAPV5HEADER",
        _ => throw new ArgumentException($"Unexpected header size {HeaderSize}", nameof(HeaderSize))
    };
    public int HeaderSize { get; set; }
    public int BitmapWidth { get; set; }
    public int BitmapHeight { get; set; }
    public short ColorPlanes { get; set; } // must be 1
    public short BitsPerPixel { get; set; } // 1, 4, 8, 16, 24, 32
    public CompressionMethod CompressionMethod { get; set; }
    public int RawImageSize { get; set; } // can be set to 0
    public int HorizontalResolution { get; set; } // ppm
    public int VerticalResolution { get; set; } // ppm 
    public int ColorPalette { get; set; } // 0 for all
    public int ImportantColors { get; set; }
    public int RedMask { get; set; }
    public int GreenMask { get; set; }
    public int BlueMask { get; set; }
    public int AlphaMask { get; set; }
    public int CSType { get; set; }
    public XYZTriple Endpoints { get; set; }
    public int GammaRed { get; set; }
    public int GammaGreen { get; set; }
    public int GammaBlue { get; set; }
    public int Intent { get; set; }
    public int ProfileData { get; set; }
    public int ProfileSize { get; set; }
    public int Reserved { get; set; }
}

public class BmpLoader : IImageLoader
{
    private static byte[] GetBytesByBPPIndex(byte[] rowBytes, int index, int bitsPerPixel)
    {
        var adjustedIndex = (int)Math.Floor(index * bitsPerPixel / 8.0);
        var width = (int)Math.Ceiling(bitsPerPixel / 8.0);

        var result = rowBytes[adjustedIndex..(adjustedIndex + width)];
        if (bitsPerPixel is 1 or 2 or 4)
        {
            var pixelsPerByte = 8 / bitsPerPixel;
            var subIndex = index % pixelsPerByte;
            var shift = bitsPerPixel * (pixelsPerByte - subIndex - 1);
            var mask = bitsPerPixel switch { 1 => 1, 2 => 3, 4 => 15, _ => throw new ArgumentException(nameof(bitsPerPixel)) };
            result[0] = (byte)((result[0] >> shift) & mask);
        }

        var padded = result.PadLeft(4, (byte) 0);
        if (BitConverter.IsLittleEndian)
            padded = padded.Reverse();
        return padded.ToArray();
    }

    private static int GetShift(int mask)
    {
        int c = 32; // c will be the number of zero bits on the right
        mask &= -mask;
        if (mask != 0) c--;
        if ((mask & 0x0000FFFF) != 0) c -= 16;
        if ((mask & 0x00FF00FF) != 0) c -= 8;
        if ((mask & 0x0F0F0F0F) != 0) c -= 4;
        if ((mask & 0x33333333) != 0) c -= 2;
        if ((mask & 0x55555555) != 0) c -= 1;
        return c;
    }
    
    private static Pixel GetColor(byte[] rowBytes, int column, int row, BitmapInfoHeader header, Pixel[] colorTable)
    {
        var pixelValue = BitConverter.ToInt32(GetBytesByBPPIndex(rowBytes, column, header.BitsPerPixel));
        if (header.CompressionMethod == CompressionMethod.BI_BITFIELDS)
        {
            return new Pixel {
                R = (byte)((pixelValue & header.RedMask) >> GetShift(header.RedMask)),
                G = (byte)((pixelValue & header.GreenMask) >> GetShift(header.GreenMask)),
                B = (byte)((pixelValue & header.BlueMask) >> GetShift(header.BlueMask)),
                A = (byte)((pixelValue & header.AlphaMask) >> GetShift(header.AlphaMask)),
                X = column,
                Y = row
            };
        }
        return header.BitsPerPixel switch
        {
            <= 8 => new Pixel
            {
                R = colorTable[pixelValue].R,
                G = colorTable[pixelValue].G,
                B = colorTable[pixelValue].B,
                X = column,
                Y = row
            },
            16 => new Pixel
            {
                R = (byte) ((pixelValue >> 8) & 15).Remap(0, 15, 0, 255).Floor(),
                G = (byte) ((pixelValue >> 4) & 15).Remap(0, 15, 0, 255).Floor(),
                B = (byte) (pixelValue & 15).Remap(0, 15, 0, 255).Floor(),
                A = (byte) ((pixelValue >> 12) & 15).Remap(0, 15, 0, 255).Floor(),
                X = column,
                Y = row,
            },
            24 => // 88800 RGBAX
                new Pixel
                {
                    R = (byte) (pixelValue & 255),
                    G = (byte) ((pixelValue >> 8) & 255),
                    B = (byte) ((pixelValue >> 16) & 255),
                    A = 255,
                    X = column,
                    Y = row,
                },
            32 => // 8888
                new Pixel
                {
                    R = (byte) ((pixelValue >> 16) & 255),
                    G = (byte) ((pixelValue >> 8) & 255),
                    B = (byte) (pixelValue & 255),
                    A = (byte) ((pixelValue >> 24) & 255),
                    X = column,
                    Y = row,
                },
            _ => throw new ArgumentOutOfRangeException(nameof(header.BitsPerPixel), "BitsPerPixel must be 1, 2, 4, 8, 16, 24, or 32.")
        };
    }
    
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
        
        if (parsedDibHeader.HeaderSize == 124) 
        {
            parsedDibHeader.RedMask = BitConverter.ToInt32(dibHeader[40..44]);
            parsedDibHeader.GreenMask = BitConverter.ToInt32(dibHeader[44..48]);
            parsedDibHeader.BlueMask = BitConverter.ToInt32(dibHeader[48..52]);
            parsedDibHeader.AlphaMask = BitConverter.ToInt32(dibHeader[52..56]);
            parsedDibHeader.CSType = BitConverter.ToInt32(dibHeader[56..60]);
            parsedDibHeader.Endpoints = new()
            {
                RedX = BitConverter.ToInt32(dibHeader[60..64]),
                RedY = BitConverter.ToInt32(dibHeader[64..68]),
                RedZ = BitConverter.ToInt32(dibHeader[68..72]),
                GreenX = BitConverter.ToInt32(dibHeader[72..76]),
                GreenY = BitConverter.ToInt32(dibHeader[76..80]),
                GreenZ = BitConverter.ToInt32(dibHeader[80..84]),
                BlueX = BitConverter.ToInt32(dibHeader[84..88]),
                BlueY = BitConverter.ToInt32(dibHeader[88..92]),
                BlueZ = BitConverter.ToInt32(dibHeader[92..96]),
            };
            parsedDibHeader.GammaRed = BitConverter.ToInt32(dibHeader[96..100]);
            parsedDibHeader.GammaGreen = BitConverter.ToInt32(dibHeader[100..104]);
            parsedDibHeader.GammaBlue = BitConverter.ToInt32(dibHeader[104..108]);
            parsedDibHeader.Intent = BitConverter.ToInt32(dibHeader[108..112]);
            parsedDibHeader.ProfileData = BitConverter.ToInt32(dibHeader[112..116]);
            parsedDibHeader.ProfileSize = BitConverter.ToInt32(dibHeader[116..120]);
            parsedDibHeader.Reserved = BitConverter.ToInt32(dibHeader[120..124]);
        } 
        else if (parsedDibHeader.HeaderSize != 40)
        {
            throw new NotImplementedException($"I have not implemented header '{parsedDibHeader.HeaderType}'");
        }

        Console.Out.WriteLine($"{parsedFileHeader.MagicNumber}");
        Console.Out.WriteLine($"{parsedFileHeader.FileSize}");
        Console.Out.WriteLine($"{parsedFileHeader.PixelArrayOffset}");
        Console.Out.WriteLine($"Width: {parsedDibHeader.BitmapWidth}");
        Console.Out.WriteLine($"Height: {parsedDibHeader.BitmapHeight}");
        Console.Out.WriteLine(JsonConvert.SerializeObject(parsedDibHeader, Formatting.Indented, new JsonConverter[] {new StringEnumConverter()}));

        // There has to be a better way
        if (parsedDibHeader.CompressionMethod is not CompressionMethod.BI_RGB and not CompressionMethod.BI_CMYK and not CompressionMethod.BI_BITFIELDS)
        {
            throw new NotImplementedException(
                $"I have not implemented compression methods other than BI_RGB, BI_CMYK, and BI_BITFIELDS for bitmap files :( ---- Found Compression Method {parsedDibHeader.CompressionMethod}");
        }
        
        // color table
        var colorTable = new Pixel[parsedDibHeader.ColorPalette];
        var colorTableOffset = 14 + parsedDibHeader.HeaderSize;
        var colorTableEntrySize = 4; // TODO handle other sized color table entries. For now assume RGBA32 8888
        if (parsedDibHeader.BitsPerPixel <= 8)
        {
            for (var i = 0; i < parsedDibHeader.ColorPalette; i++)
            {
                var color = imageFile.ReadBytes(colorTableOffset + i * colorTableEntrySize, colorTableEntrySize);
                colorTable[i] = new Pixel { R = color[0], G = color[1], B = color[2], A = color[3] };
            }
        }
        
        var resultImage = new Image
        {
            Format = FileFormat.BMP,
            Width = (uint) parsedDibHeader.BitmapWidth,
            Height = (uint) Math.Abs(parsedDibHeader.BitmapHeight),
            Pixels = new Pixel[parsedDibHeader.BitmapWidth, Math.Abs(parsedDibHeader.BitmapHeight)]
        };

        var rowSize = (int)Math.Ceiling(parsedDibHeader.BitsPerPixel * parsedDibHeader.BitmapWidth / 32.0) * 4;
        
        for (var y = 0; y < Math.Abs(parsedDibHeader.BitmapHeight); y++)
        {
            var rowBytes = imageFile.ReadBytes(parsedFileHeader.PixelArrayOffset + rowSize * y, rowSize);
            var rowIndex = parsedDibHeader.BitmapHeight < 0 ? y : parsedDibHeader.BitmapHeight - y - 1;
            for (var x = 0; x < parsedDibHeader.BitmapWidth; x++)
            {
                resultImage.Pixels[x, rowIndex] = GetColor(rowBytes, x, rowIndex, parsedDibHeader, colorTable);
            }
        }
        
        return resultImage;
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