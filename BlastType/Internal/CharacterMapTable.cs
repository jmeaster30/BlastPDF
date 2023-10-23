using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class CharacterMapTable : IFontTable
{
    public ushort Version { get; set; }
    public ushort NumberOfTables { get; set; }
    public List<EncodingRecord> EncodingRecords { get; set; } = new();

    public static CharacterMapTable Load(Stream stream)
    {
        var tableStart = stream.Position;
        var cmap = new CharacterMapTable
        {
            Version = stream.ReadU16(),
            NumberOfTables = stream.ReadU16(),
        };

        for (var i = 0; i < cmap.NumberOfTables; i++)
        {
            cmap.EncodingRecords.Add(EncodingRecord.Load(stream, tableStart));
        }

        return cmap;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(CharacterMapTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}