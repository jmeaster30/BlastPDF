using MyLib.Streams;

namespace BlastType.Internal.CompactFontFormatSubtables;

public class CompactFontFormatHeader
{
    public byte MajorVersion { get; set; }
    public byte MinorVersion { get; set; }
    public byte HeaderSize { get; set; }
    public ushort TopDictLength { get; set; }

    public static CompactFontFormatHeader Load(Stream stream)
    {
        return new CompactFontFormatHeader
        {
            // TODO fix the readbyte call so we can just call that function directly
            MajorVersion = stream.ReadBytes(1)[0],
            MinorVersion = stream.ReadBytes(1)[0],
            HeaderSize = stream.ReadBytes(1)[0],
            TopDictLength = stream.ReadU16()
        };
    }
}