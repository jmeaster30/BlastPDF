using System;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Lists;

namespace BlastPDF.Filter.Implementations;

public class Ascii85 : IFilterAlgorithm
{
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        var chunked = input.Chunk(4);
        var results = new List<byte>();
        foreach (var chunk in chunked)
        {
            var padded = chunk.PadRight(4, (byte)0);
            if (BitConverter.IsLittleEndian)
                padded = padded.Reverse();
            var value = BitConverter.ToUInt32(padded.ToArray());
            if (value == 0) return new byte[]{122};
            var fixedChunk = new byte[5];
            var i = 4;
            while (value > 0)
            {
                fixedChunk[i--] = (byte)(value % 85 + 33);
                value /= 85;
            }
            results.AddRange(fixedChunk.Take(chunk.Length + 1));
        }

        return results.Concat(new[]{(byte)'~', (byte)'>'});
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        var filtered = input.Where(x => !char.IsWhiteSpace((char)x));
        if (System.Text.Encoding.ASCII.GetString(filtered.Skip(filtered.Count() - 2).Take(2).ToArray()) != "~>")
        {
            throw new ArgumentException("Sequence must end in a '~>'.", nameof(input));
        }
        
        var contents = filtered.Take(filtered.Count() - 2).ToList();
        if (contents.Any(x => x is not (>= 33 and <= 117) or 122))
        {
            throw new ArgumentException("Sequence must contain only characters between '!' and 'u'; and 'z'");
        }
        
        var results = new List<byte>();
        var contentIndex = 0;

        while (contentIndex < contents.Count)
        {
            List<byte> toAdd;
            if (contents[contentIndex] == 'z')
            {
                toAdd = new List<byte>{0, 0, 0, 0};
                contentIndex += 1;
            }
            else
            {
                var chunk = contents.Skip(contentIndex).Take(5);
                var padded = chunk.Select(x => (byte)(x - 33)).PadRight(5, (byte)84);
                var value = padded.Aggregate((uint)0, (total, next) => 85 * total + next);
                toAdd = BitConverter.GetBytes(value).ToList();
                if (BitConverter.IsLittleEndian)
                    toAdd.Reverse();
                toAdd = toAdd.Take(chunk.Count() - 1).ToList();
                contentIndex += chunk.Count();
            }
            results.AddRange(toAdd);
        }
        
        return results;;
    }
}