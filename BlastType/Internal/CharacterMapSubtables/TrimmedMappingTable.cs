using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 6
public class TrimmedMappingTable : ICharacterMapSubtable
{
    public ushort Format { get; set; }
    public ushort Length { get; set; }
    public ushort Language { get; set; }
    public ushort FirstCode { get; set; }
    public ushort EntryCount { get; set; }
    public List<ushort> GlyphIdArray { get; set; } = new();

    public static TrimmedMappingTable Load(Stream stream)
    {
        var mappingTable = new TrimmedMappingTable
        {
            Format = stream.ReadU16(),
            Length = stream.ReadU16(),
            Language = stream.ReadU16(),
            FirstCode = stream.ReadU16(),
            EntryCount = stream.ReadU16()
        };

        for (var i = 0; i < mappingTable.EntryCount; i++)
        {
            mappingTable.GlyphIdArray.Add(stream.ReadU16());
        }

        return mappingTable;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(TrimmedMappingTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}