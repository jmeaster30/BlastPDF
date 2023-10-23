using MyLib.Streams;

namespace BlastType.Internal.GlyphDefinitionSubtables;

public class GlyphClassDefinitionTable
{
    public ushort ClassFormat { get; set; }
    
    // Format 1
    public ushort StartGlyphId { get; set; }
    public ushort GlyphCount { get; set; }
    public List<ushort> ClassValueArray { get; set; } = new();

    // Format 2
    public ushort ClassRangeCount { get; set; }
    public List<ClassRangeRecord> ClassRangeRecords { get; set; } = new();

    public static GlyphClassDefinitionTable Load(Stream stream)
    {
        var gcd = new GlyphClassDefinitionTable
        {
            ClassFormat = stream.ReadU16(),
        };

        if (gcd.ClassFormat == 1)
        {
            gcd.StartGlyphId = stream.ReadU16();
            gcd.GlyphCount = stream.ReadU16();
            for (var i = 0; i < gcd.GlyphCount; i++)
            {
                gcd.ClassValueArray.Add(stream.ReadU16());
            }
        }
        else if (gcd.ClassFormat == 2)
        {
            gcd.ClassRangeCount = stream.ReadU16();
            for (var i = 0; i < gcd.ClassRangeCount; i++)
            {
                gcd.ClassRangeRecords.Add(ClassRangeRecord.Load(stream));
            }
        }

        return gcd;
    }
}