using MyLib.Streams;

namespace BlastType.Internal;

public class NameRecord
{
    public ushort PlatformId { get; set; }
    public ushort EncodingId { get; set; }
    public ushort LanguageId { get; set; }
    public ushort NameId { get; set; }
    public ushort Length { get; set; }
    public ushort Offset { get; set; }
    // This is supposed to be on the NameTable but each record is supposed to have a piece of that overall name data
    public byte[] NameData { get; set; } = Array.Empty<byte>();

    public static NameRecord Load(Stream stream, long startOfTable, NameTable nameTable)
    {
        var record = new NameRecord
        {
            PlatformId = stream.ReadU16(),
            EncodingId = stream.ReadU16(),
            LanguageId = stream.ReadU16(),
            NameId = stream.ReadU16(),
            Length = stream.ReadU16(),
            Offset = stream.ReadU16(),
        };
        var endOfRecord = stream.Position;
        stream.Seek(startOfTable + nameTable.StringOffset + record.Offset, SeekOrigin.Begin);
        record.NameData = stream.ReadBytes(record.Length);
        stream.Seek(endOfRecord, SeekOrigin.Begin);
        return record;
    }
}