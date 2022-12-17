using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace BlastPDF.Builder;

public enum PdfFilter
{
    ASCIIHexDecode,
    ASCII85Decode,
    LZWDecode,
    FlateDeode,
    RunLengthDecode,
    CCITTFaxDecode,
    JBIG2Decode,
    DCTDecode,
    JPXDecode,
    Crypt,
}

public static class PdfFilterExtensions
{
    private static byte ToHex(int b)
    {
        if (b is < 0 or > 15)
            throw new ArgumentOutOfRangeException(nameof(b), b, "Value must be greater than 0 and less than 16");
        return (byte)(b < 10 ? b + 48 : b + 55);
    }

    private static IEnumerable<T> PadRight<T>(this IEnumerable<T> a, int amount, T value)
    {
        int toAdd = amount - a.Count();
        if (toAdd <= 0) return a;
        for (int i = 0; i < toAdd; i++)
        {
            a = a.Append(value);
        }
        
        return a;
    }

    public static IEnumerable<byte> ASCIIHexEncode(this IEnumerable<byte> input)
    {
        return input.SelectMany(x => new List<byte> {ToHex(x / 16), ToHex(x % 16)});
    }

    public static IEnumerable<byte> ASCII85Encode(this IEnumerable<byte> input)
    {
        var chunks = input.Chunk(4);
        var paddingAmount = 4 - input.Count() % 4;
        var results = new List<byte>();

        foreach (var chunk in chunks)
        {
            var padded = chunk.PadRight(4, (byte) 0);
            if (BitConverter.IsLittleEndian)
                padded = padded.Reverse();
            var value = BitConverter.ToUInt32(padded.ToArray());

            if (value == 0)
            {
                results.AddRange(new List<byte> {33, 33, 33, 33, 33});
                continue;
            }

            var fixedChunk = new byte[5];
            var i = 4;
            while (value > 0)
            {
                fixedChunk[i--] = (byte)(value % 85 + 33);
                value /= 85;
            }
            results.AddRange(fixedChunk);
        }

        return results.Take(results.Count - paddingAmount);
    }
    
    
}