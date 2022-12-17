using BlastSharp.Streams;

namespace BlastSharp;

public class BlastSharp
{
    public static Stream FromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static void PrintContents(string name, Stream s)
    {
        Console.Write($"{name}: '");
        s.Position = 0;
        while (s.Position < s.Length)
        {
            Console.Write(Convert.ToChar((byte)s.ReadByte()));
        }
        Console.WriteLine("'");
    }
    
    public static void Main(string[] args)
    {
        var stream = FromString("This is my stream :)");
        var filtered = stream.Filter(x => x is >= 97 and <= 122);
        var readTransform = stream.TransformOnRead(x => x == 32 ? (byte) 126 : x);

        var baseStream = new MemoryStream();
        var writeTransform = baseStream.TransformOnWrite(x => (byte)(x + 1));
        var writer = new StreamWriter(writeTransform);
        writer.Write("This is my stream :)");
        writer.Flush();
        stream.Position = 0;

        var chained = writeTransform.Filter(x => x != '!').TransformOnRead(x => (byte) (x - 1));
        
        PrintContents("Original", stream);
        PrintContents("Filtered", filtered);
        PrintContents("ReadTransform", readTransform);
        PrintContents("BaseStream", baseStream);
        PrintContents("WriteTransform", writeTransform);
        PrintContents("Chained", chained);
    }
}