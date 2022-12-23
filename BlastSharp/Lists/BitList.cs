using System.Collections;

namespace BlastSharp.Lists;

public class BitList : IEnumerable
{
    private List<bool> Contents { get; set; }

    public IEnumerator GetEnumerator()
    {
        return Contents.GetEnumerator();
    }

    public void AppendBits(int value, int bitLength)
    {
        for (int shift = bitLength - 1; shift >= 0; shift--)
        {
            Contents.Append(((value >> shift) & 1) == 1);
        }
    }

    public void RemoveBits(int amount)
    {
        Contents.RemoveRange(Contents.Count - amount, amount);
    }

    public byte[] ToByteArray()
    {
        return Contents.Chunk(8)
            .Select(x => x.Length < 8 ? x.PadRight(8, false) : x)
            .Select(source =>
            {
                byte result = 0;
                var index = 8 - source.Count();
                foreach (var b in source)
                {
                    if (b) result |= (byte)(1 << (7 - index));
                    index++;
                }
                return result;
            }).ToArray();
    }
}