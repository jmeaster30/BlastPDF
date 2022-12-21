using System.Text;
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
        Console.WriteLine($"Ascii 85 Encode/Decode: 'result'");
    }
}