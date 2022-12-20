using BlastSharp.Lists;

namespace BlastSharp.Compression;

public class RunLength : ICompressionAlgorithm
{
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
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
            var current = chunks.LastOrDefault();
            if(chunks.Count > 0 && g.Item2 == 1 && current.Item2 == 1 && current.Item1.Count() < 128)
            {
                current.Item1 = current.Item1.Append(g.Item1);
            }
            else
            {
                chunks.Add((new []{g.Item1}, g.Item2));
            }
        }

        return chunks.SelectMany(x =>
            x.Item1.Prepend(x.Item2 == 1 ? (byte) (x.Item1.Count() - 1) : (byte) (257 - x.Item2))).Append((byte)128);
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        throw new NotImplementedException();
    }
}