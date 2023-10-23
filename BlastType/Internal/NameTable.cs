using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class NameTable : IFontTable
{
    public ushort Format { get; set; }
    public ushort Count { get; set; }
    public ushort StringOffset { get; set; }
    public List<NameRecord> NameRecords { get; set; } = new();
    public ushort LanguageTagCount { get; set; }
    public List<LanguageRecord> LanguageRecords { get; set; } = new();

    public static NameTable Load(Stream stream)
    {
        var startOfTable = stream.Position;
        var nameTable = new NameTable
        {
            Format = stream.ReadU16(),
            Count = stream.ReadU16(),
            StringOffset = stream.ReadU16(),
        };

        for (var i = 0; i < nameTable.Count; i++)
        {
            nameTable.NameRecords.Add(NameRecord.Load(stream, startOfTable, nameTable));
        }

        if (nameTable.Format == 1)
        {
            nameTable.LanguageTagCount = stream.ReadU16();
            for (var i = 0; i < nameTable.LanguageTagCount; i++)
            {
                nameTable.LanguageRecords.Add(LanguageRecord.Load(stream, startOfTable, nameTable));
            }
        }

        return nameTable;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(NameTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}