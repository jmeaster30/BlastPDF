using System.Runtime.InteropServices;

namespace BlastSharp.Streams;

public class FilteredStream : Stream
{
    public FilteredStream(Stream contents, Func<byte, bool> filter)
    {
        _contents = contents;
        _filterFunction = filter;
    }
    
    public override void Flush()
    {
        _contents.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Value must be non-negative");
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Value must be non-negative");
        if (buffer.Length - offset < count) throw new ArgumentException("Invalid Offset Length");
        if (_contents.Length - _contents.Position <= 0) return 0;
        
        var requestedEnd = count;
        var bytesRead = 0;
        var bytesFiltered = 0;
        while (_contents.Position < _contents.Length && bytesRead < requestedEnd)
        {
            var r = _contents.ReadByte();
            if (r < 0)
            {
                break;
            }
            if (_filterFunction((byte) r))
            {
                if (bytesRead >= offset)
                {
                    buffer[offset + bytesFiltered] = (byte)r;
                    bytesFiltered += 1;
                }
            }
            else if (bytesRead >= offset)
            {
                requestedEnd += 1;
            } 
            bytesRead++;
        }

        if (bytesFiltered == 0) return -1;
        return bytesFiltered;
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
        _contents.Write(buffer.Where(_filterFunction).ToArray(), offset, count);
    }
    
    private Stream _contents { get; }
    private Func<byte, bool> _filterFunction { get; }

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
