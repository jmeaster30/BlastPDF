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

    public static IEnumerable<byte> Encode(this IEnumerable<byte> input, PdfFilter decodeType)
    {
        return decodeType switch
        {
            PdfFilter.ASCIIHexDecode => input.ASCIIHexEncode(),
            PdfFilter.ASCII85Decode => input.ASCII85Encode(),
            PdfFilter.LZWDecode => throw new NotImplementedException(),
            PdfFilter.FlateDeode => throw new NotImplementedException(),
            PdfFilter.RunLengthDecode => input.RunLengthEncode(),
            PdfFilter.CCITTFaxDecode => throw new NotImplementedException(),
            PdfFilter.JBIG2Decode => throw new NotImplementedException(),
            PdfFilter.DCTDecode => throw new NotImplementedException(),
            PdfFilter.JPXDecode => throw new NotImplementedException(),
            PdfFilter.Crypt => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(decodeType), decodeType, "Unsupported PDF Filter :(")
        };
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

    public static IEnumerable<byte> RunLengthEncode(this IEnumerable<byte> input)
    {
        // decoding
        // length byte -> run
        // if length is 0 to 127 then copy next length + 1 bytes literally
        // if length is 129 to 255 then the next single byte should be copied 257 - length times
        // length of 128 is EOD
        
        // encoding
        // group same bytes into groups of (byte value, run length)
        // group groups of run length = 1 into chunks of 128
        // if run length is greater than 1
            // emit {257 - run length, byte value}
        // if run length is equal to 1
            // emit {length, chunked bytes}
        // add byte 128 to end

        var groups = input.RunGroupBy(x => x, 128);
        
        var chunks = new List<(IEnumerable<byte>, int)>();
        foreach (var g in groups)
        {
            Console.WriteLine($"byte: {Convert.ToInt32(g.Item1)} run length: {g.Item2}");
            var current = chunks.LastOrDefault();
            if(chunks.Count > 0 && g.Item2 == 1 && current.Item2 == 1 && current.Item1.Count() < 128)
            {
                Console.WriteLine("Append");
                current.Item1 = current.Item1.Append(g.Item1);
            }
            else
            {
                Console.WriteLine("New Chunk");
                chunks.Add((new []{g.Item1}, g.Item2));
            }
        }

        return chunks.SelectMany(x =>
            x.Item1.Prepend(x.Item2 == 1 ? (byte) (x.Item1.Count() - 1) : (byte) (257 - x.Item2))).Append((byte)128);
    }
}