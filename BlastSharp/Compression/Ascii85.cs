using System.ComponentModel;
using System.Text;
using BlastSharp.Lists;

namespace BlastSharp.Compression;

public class Ascii85 : ICompressionAlgorithm
{
    public IEnumerable<byte> Encode(IEnumerable<byte> input)
    {
        var results = input.Chunk(4)
            .Select(x => {
                var padded = x.PadRight(4, (byte)0);
                if (BitConverter.IsLittleEndian)
                    padded = padded.Reverse();
                return BitConverter.ToUInt32(padded.ToArray());
            }).SelectMany(value => {
                if (value == 0) return new byte[]{122};
                var fixedChunk = new byte[5];
                var i = 4;
                while (value > 0)
                {
                    fixedChunk[i--] = (byte)(value % 85 + 33);
                    value /= 85;
                }
                return fixedChunk;
            });
        
        return results.Take(results.Count() - (4 - input.Count() % 4)).Concat(new[]{(byte)'~', (byte)'>'});
    }

    public IEnumerable<byte> Decode(IEnumerable<byte> input)
    {
        var filtered = input.Where(x => !char.IsWhiteSpace((char)x));
        if (Encoding.ASCII.GetString(filtered.Skip(filtered.Count() - 2).Take(2).ToArray()) != "~>")
        {
            throw new ArgumentException("Sequence must end in a '~>'.", nameof(input));
        }
        
        var contents = filtered.Take(filtered.Count() - 2);
        if (contents.Any(x => x is not (>= 33 and <= 117) or 122))
        {
            throw new ArgumentException("Sequence must contain only characters between '!' and 'u'; and 'z'");
        }

        var results = contents.Chunk(5)
            .Select(x => x.Select(y => (byte)(y - 33)).PadRight(5, (byte)84)
                .Aggregate((uint)0, (total, next) => 85 * total + next))
            .SelectMany(x =>
            {
                var bytes = BitConverter.GetBytes(x).ToList();
                if (BitConverter.IsLittleEndian)
                    bytes.Reverse();
                return bytes;
            });

        return results.Take(results.Count() - input.Count() % 5 + 1);
    }
}