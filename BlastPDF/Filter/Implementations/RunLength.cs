using System;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Lists;

namespace BlastPDF.Filter.Implementations;

public class RunLength : IFilterAlgorithm
{
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        // encoding
        // group same bytes into groups of (byte value, run length)
        // group groups of run length = 1 into chunks of 128
        // if run length is greater than 1
        // emit {257 - run length, byte value}
        // if run length is equal to 1
        // emit {length, chunked bytes}
        // add byte 128 to end

        var groups = new List<(byte, int)>();
        (byte, int) currentGroup = (0, 0);
        foreach (var i in input)
        {
            if (currentGroup.Item2 != 0 && currentGroup.Item1  == i && currentGroup.Item2 < 128)
            {
                currentGroup = (currentGroup.Item1, currentGroup.Item2 + 1);
            }
            else
            {
                if (currentGroup.Item2 > 0) groups.Add(currentGroup);
                currentGroup = (i, 1);
            }
        }
        if (currentGroup.Item2 > 0) groups.Add(currentGroup);
        
        var chunks = new List<(IEnumerable<byte>, int)>();
        var currentChunk = (new List<byte>(), 0);
        foreach (var g in groups)
        {
            if(currentChunk.Item2 != 0 && g.Item2 == 1 && currentChunk.Item2 == 1 && currentChunk.Item1.Count < 128)
            {
                var a = currentChunk.Item1;
                a.Add(g.Item1);
                currentChunk = (a, currentChunk.Item2);
            }
            else
            {
                if (currentChunk.Item2 > 0) chunks.Add(currentChunk);
                currentChunk = (new List<byte>{g.Item1}, g.Item2);
            }
        }
        if (currentChunk.Item2 > 0) chunks.Add(currentChunk);

        return chunks.SelectMany(x => x.Item1
            .Prepend(x.Item2 == 1 ? (byte) (x.Item1.Count() - 1) : (byte) (257 - x.Item2)))
            .Append((byte)128);
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        // decoding
        // length byte -> run
        // if length is 0 to 127 then copy next length + 1 bytes literally
        // if length is 129 to 255 then the next single byte should be copied 257 - length times
        // length of 128 is EOD
        var result = new List<byte>();
        for(var i = 0; i < input.Count(); i++)
        {
            var b = input.ElementAt(i);
            if (b < 128)
            {
                result.AddRange(input.Skip(i + 1).Take(b + 1));
                i += b + 1;
            }
            else if (b > 128)
            {
                result.AddRange(input.ElementAt(i + 1).Repeat(257 - b));
                i += 1;
            }
            else
            {
                break;
            }
        }

        return result;
    }
}