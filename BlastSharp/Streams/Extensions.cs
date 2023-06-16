using System.Text;

namespace BlastSharp.Streams;

public static class Extensions
{
    public static FilteredStream Filter(this Stream stream, Func<byte, bool> filter)
    {
        return new FilteredStream(stream, filter);
    }
    
    public static ReadTransformStream TransformOnRead(this Stream stream, Func<byte, byte> process)
    {
        return new ReadTransformStream(stream, process);
    }

    public static WriteTransformStream TransformOnWrite(this Stream stream, Func<byte, byte> process)
    {
        return new WriteTransformStream(stream, process);
    }

    public static ushort ReadU16(this Stream stream)
    {
        var bytes = new byte[] { 0, 0 };
        var bytesRead = stream.Read(bytes);
        if (bytesRead != 2) throw new IndexOutOfRangeException();
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reverse().ToArray();
        }
        return BitConverter.ToUInt16(bytes);
    }
    
    public static uint ReadU32(this Stream stream)
    {
        var bytes = new byte[] { 0, 0, 0, 0 };
        var bytesRead = stream.Read(bytes);
        if (bytesRead != 4) throw new IndexOutOfRangeException();
        if (BitConverter.IsLittleEndian)
        {
            bytes = bytes.Reverse().ToArray();
        }
        return BitConverter.ToUInt32(bytes);
    }

    public static byte[] ReadBytes(this Stream stream, int length)
    {
        var bytes = new byte[length];
        var bytesRead = stream.Read(bytes);
        if (bytesRead != 4) throw new IndexOutOfRangeException();
        return bytes;
    }

    public static string ReadString(this Stream stream, int length)
    {
        var bytes = new byte[length];
        var bytesRead = stream.Read(bytes);
        if (bytesRead != 4) throw new IndexOutOfRangeException();
        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
    }
}