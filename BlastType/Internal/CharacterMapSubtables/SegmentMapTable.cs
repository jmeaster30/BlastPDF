using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap subtable format 4
public class SegmentMapTable : ICharacterMapSubtable
{
    public ushort Format { get; set; }
    public ushort Length { get; set; }
    public ushort Language { get; set; }
    public ushort SegmentCountX2 { get; set; }
    public ushort SearchRange { get; set; }
    public ushort EntrySelector { get; set; }
    public ushort RangeShift { get; set; }
    public List<ushort> EndCode { get; set; } = new();
    public ushort ReservedPad { get; set; }
    public List<ushort> StartCode { get; set; } = new();
    public List<short> IdDelta { get; set; } = new();
    public List<ushort> IdRangeOffset { get; set; } = new();
    public List<ushort> GlyphIdArray { get; set; } = new();

    public static SegmentMapTable Load(Stream stream)
    {
        var startOfTable = stream.Position;
        var segmentMapTable = new SegmentMapTable
        {
            Format = stream.ReadU16(),
            Length = stream.ReadU16(),
            Language = stream.ReadU16(),
            SegmentCountX2 = stream.ReadU16(),
            SearchRange = stream.ReadU16(),
            EntrySelector = stream.ReadU16(),
            RangeShift = stream.ReadU16()
        };

        var segmentCount = segmentMapTable.SegmentCountX2 / 2;
        for (var i = 0; i < segmentCount; i++)
        {
            segmentMapTable.EndCode.Add(stream.ReadU16());
        }

        segmentMapTable.ReservedPad = stream.ReadU16();
        
        for (var i = 0; i < segmentCount; i++)
        {
            segmentMapTable.StartCode.Add(stream.ReadU16());
        }
        
        for (var i = 0; i < segmentCount; i++)
        {
            segmentMapTable.IdDelta.Add(stream.ReadS16());
        }
        
        for (var i = 0; i < segmentCount; i++)
        {
            segmentMapTable.IdRangeOffset.Add(stream.ReadU16());
        }

        var endMarker = stream.Position;

        for (var i = 0; i < (segmentMapTable.Length - (endMarker - startOfTable)) / 2; i++)
        {
            segmentMapTable.GlyphIdArray.Add(stream.ReadU16());
        }

        return segmentMapTable;
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(SegmentMapTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    
}