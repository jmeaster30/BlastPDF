using System.Collections;
using BlastSharp.Numbers;

namespace BlastSharp.Lists;

public class BitList : IEnumerable
{
    private List<bool> Contents { get; set; } = new();

    public BitList()
    {}
    
    public BitList(IEnumerable<byte> contents)
    {
        Contents = contents.SelectMany(x =>
        {
            var res = new List<bool>();
            for (int i = 7; i >= 0; i--)
            {
                res.Add(((x >> i) & 1) == 1);
            }
            return res;
        }).ToList();
    }

    public int Count => Contents.Count;

    public IEnumerator GetEnumerator()
    {
        return Contents.GetEnumerator();
    }

    public void AppendBits(int value, int bitLength)
    {
        for (int shift = bitLength - 1; shift >= 0; shift--)
        {
            Contents.Add(((value >> shift) & 1) == 1);
        }
    }

    public void RemoveBits(int amount)
    {
        Contents.RemoveRange(Contents.Count - amount, amount);
    }

    public byte[] ReadBits(int index, int count)
    {
        try
        {
            var value = Contents.GetRange(index, count);
            return ToByteArray(value.PadLeft((value.Count / 8.0).Ceiling() * 8, false));
        }
        catch
        {
            Console.WriteLine($"Count: {Contents.Count} Offset: {index} Amount: {count}");
            throw;
        }
    }

    public byte[] ToByteArray()
    {
        return ToByteArray(Contents);
    }
    
    private static byte[] ToByteArray(IEnumerable<bool> value)
    {
        return value.PadRight((value.Count() / 8.0).Ceiling() * 8, false)
            .Chunk(8).Select(source =>
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

public static class BitListExtensions {
    public static BitList ToBitList(this IEnumerable<byte> contents)
    {
        return new BitList(contents);
    }
}