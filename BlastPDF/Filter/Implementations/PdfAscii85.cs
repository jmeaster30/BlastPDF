using System;
using System.Collections.Generic;
using System.Linq;
using MyLib.Compression;
using MyLib.Compression.Interface;
using MyLib.Enumerables;

namespace BlastPDF.Filter.Implementations;

public class PdfAscii85 : ICompressionAlgorithm
{
    private Ascii85 algorithm = new();
    
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        return algorithm.Encode(input).Concat(new[]{(byte)'~', (byte)'>'});
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        var filtered = input.Where(x => !char.IsWhiteSpace((char)x));
        if (System.Text.Encoding.ASCII.GetString(filtered.Skip(filtered.Count() - 2).Take(2).ToArray()) != "~>")
        {
            throw new ArgumentException("Sequence must end in a '~>'.", nameof(input));
        }
        
        var contents = filtered.Take(filtered.Count() - 2).ToList();
        return algorithm.Decode(contents);
    }
}