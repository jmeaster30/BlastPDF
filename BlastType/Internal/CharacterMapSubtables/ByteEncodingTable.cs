using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 0
public class ByteEncodingTable : ICharacterMapSubtable
{
    public ushort Format { get; set; }
    public ushort Length { get; set; }
    public ushort Language { get; set; }
    public byte[] GlyphIdArray { get; set; } = Array.Empty<byte>();

    public static ByteEncodingTable Load(Stream stream)
    {
        return new ByteEncodingTable
        {
            Format = stream.ReadU16(),
            Length = stream.ReadU16(),
            Language = stream.ReadU16(),
            GlyphIdArray = stream.ReadBytes(256)
        };
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(ByteEncodingTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}