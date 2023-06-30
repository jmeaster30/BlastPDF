namespace BlastIMG.ImageLoaders;

public static class ImageLoaderHelpers
{
    public static byte[] ReadBytes(this Stream stream, long fileOffset, int count)
    {
        var buffer = new byte[count];
        stream.Seek(fileOffset, SeekOrigin.Begin);
        var bytesRead = stream.Read(buffer, 0, count);
        if (bytesRead != count)
            Console.Error.WriteLine("Didn't read the correct amount of bytes");
        return buffer;
    }

    public static byte ReadByte(this Stream stream, long fileOffset)
    {
        stream.Seek(fileOffset, SeekOrigin.Begin);
        var value = stream.ReadByte();
        if (value == -1) throw new ArgumentException("File offset past end of file :(", nameof(fileOffset));
        return (byte)value;
    }

    public static uint ToU32(this byte[] bytes)
    {
        if (bytes.Length > 4) throw new ArgumentException("Too many bytes :( I only wanted 4 or less", nameof(bytes));
        uint result = 0;
        foreach (var b in bytes)
        {
            result *= 256;
            result += b;
        }

        return result;
    }
    
    public static ushort ToU16(this byte[] bytes)
    {
        if (bytes.Length > 2) throw new ArgumentException("Too many bytes :( I only wanted 2 or less", nameof(bytes));
        ushort result = 0;
        foreach (var b in bytes)
        {
            result *= 256;
            result += b;
        }

        return result;
    }

    public static byte ByteSum(this byte lhs, int rhs)
    {
        return (byte)((lhs + rhs + 256) % 256);
    }
}