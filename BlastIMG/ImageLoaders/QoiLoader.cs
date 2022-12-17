using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BlastIMG.ImageLoaders;

class QoiHeader
{
    public string MagicNumber { get; init; } = "";
    public uint Width { get; init; }
    public uint Height { get; init; }
    public byte Channels { get; init; }
    public byte ColorSpace { get; init; }
}

public class QoiLoader : IImageLoader
{
    public static Image Load(string filename)
    {
        using var imageFile = File.OpenRead(filename);

        var headerBytes = imageFile.ReadBytes(0, 14);
        var parsedHeader = new QoiHeader
        {
            MagicNumber = Encoding.Default.GetString(headerBytes[..4]),
            Width = headerBytes[4..8].ToU32(),
            Height = headerBytes[8..12].ToU32(),
            Channels = headerBytes[12],
            ColorSpace = headerBytes[13]
        };

        if (parsedHeader.MagicNumber != "qoif")
        {
            throw new ArgumentException(
                $"'{filename}' is not a QOI image. Expected magic number 'qoif' but got '{parsedHeader.MagicNumber}'",
                nameof(filename));
        }

        var pixels = new List<Pixel>();
        var index = new Pixel?[64];
        var fileOffset = 14;
        var fileLength = imageFile.Length - 8; //chop off last 8 bytes

        while (fileOffset < fileLength)
        {
            var tag = imageFile.ReadByte(fileOffset);
            var lastPixel = pixels.LastOrDefault(new Pixel(0, 0, 0, 0, 0, 255));
            if (tag == 254) // QOI_OP_RGB
            {
                var r = imageFile.ReadByte(fileOffset + 1);
                var g = imageFile.ReadByte(fileOffset + 2);
                var b = imageFile.ReadByte(fileOffset + 3);
                pixels.Add(new Pixel { R = r, G = g, B = b, A = lastPixel.A });
                fileOffset += 4;
            }
            else if (tag == 255) // QOI_OP_RGBA
            {
                var r = imageFile.ReadByte(fileOffset + 1);
                var g = imageFile.ReadByte(fileOffset + 2);
                var b = imageFile.ReadByte(fileOffset + 3);
                var a = imageFile.ReadByte(fileOffset + 4);
                pixels.Add(new Pixel { R = r, G = g, B = b, A = a });
                fileOffset += 5;
            }
            else if ((tag & 192) == 0) // QOI_OP_INDEX
            {
                pixels.Add(index[tag] ?? new Pixel(0, 0, 0, 0, 0, 0));
                fileOffset += 1;
            }
            else if ((tag & 192) == 64) // QOI_OP_DIFF
            {
                var nr = lastPixel.R.ByteSum(((tag >> 4) & 3) - 2);
                var ng = lastPixel.G.ByteSum(((tag >> 2) & 3) - 2);
                var nb = lastPixel.B.ByteSum((tag & 3) - 2);
                pixels.Add(new Pixel {R = nr, G = ng, B = nb, A = lastPixel.A });
                fileOffset += 1;
            }
            else if ((tag & 192) == 128) // QOI_OP_LUMA
            {
                var nextByte = imageFile.ReadByte(fileOffset + 1);
                var diffGreen = (tag & 63) - 32;
                var drdg = ((nextByte >> 4) & 15) - 8;
                var dbdg = (nextByte & 15) - 8;
                var nr = lastPixel.R.ByteSum(diffGreen + drdg);
                var ng = lastPixel.G.ByteSum(diffGreen);
                var nb = lastPixel.B.ByteSum(diffGreen + dbdg);
                pixels.Add(new Pixel {R = nr, G = ng, B = nb, A = lastPixel.A});
                fileOffset += 2;
            }
            else if ((tag & 192) == 192) // QOI_OP_RUN
            {
                var count = (tag & 63) + 1;
                pixels.AddRange(MakeCopies(lastPixel, count));
                fileOffset += 1;
            }

            var toIndex = pixels.LastOrDefault(new Pixel(0, 0, 0, 0, 0, 255));
            index[GetPixelHash(toIndex)] = toIndex;
        }

        var pixelData = new Pixel[parsedHeader.Width, parsedHeader.Height];
        for (var y = 0; y < parsedHeader.Height; y++)
        {
            for (var x = 0; x < parsedHeader.Width; x++)
            {
                var pixelIndex = (int)(x + y * parsedHeader.Width);
                var pixel = pixels[pixelIndex];
                pixel.X = x;
                pixel.Y = y;
                pixelData[x, y] = pixel;
            }
        }

        return new Image
        {
            Format = FileFormat.QOI,
            Width = parsedHeader.Width,
            Height = parsedHeader.Height,
            Pixels = pixelData
        };
    }

    private static int GetPixelHash(Pixel pixel)
    {
        return (pixel.R * 3 + pixel.G * 5 + pixel.B * 7 + pixel.A * 11) % 64;
    }

    private static IEnumerable<Pixel> MakeCopies(Pixel pixel, int count)
    {
        var results = new List<Pixel>();
        for (var i = 0; i < count; i++)
        {
            results.Add(pixel);
        }
        return results;
    }
}