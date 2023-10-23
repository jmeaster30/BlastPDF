using System;
using System.Collections.Generic;
using System.Linq;
using MyLib.Compression;
using MyLib.Compression.Interface;

namespace BlastPDF.Filter.Implementations;

public class PdfAsciiHex : ICompressionAlgorithm
{
    private AsciiHex algorithm = new();
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        return algorithm.Encode(input).Append((byte)'>');
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    { 
        if (input.LastOrDefault() != (byte) '>')
        {
            throw new ArgumentException("Sequence must end in a '>'.", nameof(input));
        }

        return algorithm.Decode(input.Take(input.Count() - 1));
    }
}