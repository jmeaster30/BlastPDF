using BlastType.Internal.CharacterMapSubtables;
using MyLib.Streams;

namespace BlastType.Internal;

public class EncodingRecord
{
    public ushort PlatformId { get; set; }
    public ushort EncodingId { get; set; }
    public uint Offset { get; set; }
    public ICharacterMapSubtable Subtable { get; set; }

    public static EncodingRecord Load(Stream stream, long startOfTable)
    {
        var record = new EncodingRecord
        {
            PlatformId = stream.ReadU16(),
            EncodingId = stream.ReadU16(),
            Offset = stream.ReadU32()
        };
        
        // get sub table
        var position = stream.Position;
        stream.Seek(startOfTable + record.Offset, SeekOrigin.Begin);
        record.Subtable = ICharacterMapSubtable.Load(stream);
        stream.Seek(position, SeekOrigin.Begin);
        
        return record;
    }
}