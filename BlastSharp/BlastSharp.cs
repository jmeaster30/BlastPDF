using System.Text;
using BlastSharp.Compression;
using BlastSharp.Streams;

namespace BlastSharp;

public class BlastSharp
{
    public static byte[] FromString(string s)
    {
        return Encoding.ASCII.GetBytes(s);
    }
    
    public static string ToString(IEnumerable<byte> s)
    {
        return Encoding.ASCII.GetString(s.ToArray());
    }
    
    public static void Main(string[] args)
    {
        var compressor = new Ascii85();
        var result = ToString(compressor.Decode(compressor.Encode(FromString(@"Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure."))));
        Console.WriteLine($"Ascii 85 Encode/Decode: '{result}'");
    }
}