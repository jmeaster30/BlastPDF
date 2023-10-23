using MyLib.Streams;

namespace BlastType.Internal;

public class LanguageRecord
{
    public ushort Length { get; set; }
    public ushort Offset { get; set; }
    // string data. This is supposed to be data in the NameTable entry but this language record is supposed to have a piece of the pie
    public byte[] LanguageData { get; set; } = Array.Empty<byte>();
    
    public static LanguageRecord Load(Stream stream, long startOfTable, NameTable nameTable)
    {
        var record = new LanguageRecord
        {
            Length = stream.ReadU16(),
            Offset = stream.ReadU16(),
        };
        var endOfRecord = stream.Position;
        stream.Seek(startOfTable + nameTable.StringOffset + record.Offset, SeekOrigin.Begin);
        record.LanguageData = stream.ReadBytes(record.Length);
        stream.Seek(endOfRecord, SeekOrigin.Begin);
        return record;
    }
}