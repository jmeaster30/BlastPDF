using System;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Lists;

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

    public static IEnumerable<byte> ASCIIHexEncode(this IEnumerable<byte> input)
    {
        return input.SelectMany(x => new List<byte> {ToHex(x / 16), ToHex(x % 16)});
    }

    public static IEnumerable<byte> ASCII85Encode(this IEnumerable<byte> input)
    {
        var results = input.Chunk(4)
            .Select(x => {
                var padded = x.PadRight(4, (byte)0);
                if (BitConverter.IsLittleEndian)
                    padded = padded.Reverse();
                return BitConverter.ToUInt32(padded.ToArray());
            }).SelectMany(value => {
                if (value == 0) return new byte[5]{33, 33, 33, 33, 33};
                var fixedChunk = new byte[5];
                var i = 4;
                while (value > 0)
                {
                    fixedChunk[i--] = (byte)(value % 85 + 33);
                    value /= 85;
                }
                return fixedChunk;
            });
        
        return results.Take(results.Count() - (4 - input.Count() % 4));
    }
    
    
}