using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap subtable format 8
public class MixedCoverageTable : ICharacterMapSubtable
{
    public ushort Format { get; set; }
    public ushort Reserved { get; set; }
    public uint Length { get; set; }
    public uint Language { get; set; }
    public byte[] Is32 { get; set; } = Array.Empty<byte>();
    public uint NumberOfGroups { get; set; }
    public List<SequentialMapGroup> SequentialMapGroups { get; set; } = new();

    public static MixedCoverageTable Load(Stream stream)
    {
        var mixedCoverageTable = new MixedCoverageTable
        {
            Format = stream.ReadU16(),
            Reserved = stream.ReadU16(),
            Length = stream.ReadU32(),
            Language = stream.ReadU32(),
            Is32 = stream.ReadBytes(8192),
            NumberOfGroups = stream.ReadU32(),
        };

        for (var i = 0; i < mixedCoverageTable.NumberOfGroups; i++)
        {
            mixedCoverageTable.SequentialMapGroups.Add(SequentialMapGroup.Load(stream));
        }
        
        return mixedCoverageTable;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(MixedCoverageTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}