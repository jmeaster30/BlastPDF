namespace BlastSharp.Streams;

public class ReadTransformStream : Stream
{
    public ReadTransformStream(Stream contents, Func<byte, byte> process)
    {
        _contents = contents;
        _processFunction = process;
    }
    
    public override void Flush()
    {
        _contents.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesRead = _contents.Read(buffer, offset, count);
        if (bytesRead == 0) return -1;
        for(var i = offset; i < buffer.Length; i++)
        {
            buffer[i] = _processFunction(buffer[i]);
        }
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _contents.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _contents.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _contents.Write(buffer, offset, count);
    }
    
    private Stream _contents { get; }
    private Func<byte, byte> _processFunction { get; }

    public override bool CanRead => _contents.CanRead;
    public override bool CanSeek => _contents.CanSeek;
    public override bool CanWrite => _contents.CanWrite;
    public override long Length => _contents.Length;
    public override long Position
    {
        get { return _contents.Position; }
        set { _contents.Position = value; }
    }
}
