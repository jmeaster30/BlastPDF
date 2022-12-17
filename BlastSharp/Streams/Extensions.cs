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
}