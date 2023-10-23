using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal.GlyphDefinitionSubtables;

public enum GlyphClassDef : ushort
{
    Base = 1,
    Ligature,
    Mark,
    Component
}

public class ClassRangeRecord
{
    public ushort StartGlyphId { get; set; }
    public ushort EndGlyphId { get; set; }
    public ushort Class { get; set; }
    public GlyphClassDef ClassDef => (GlyphClassDef)Class;

    public static ClassRangeRecord Load(Stream stream)
    {
        return new ClassRangeRecord
        {
            StartGlyphId = stream.ReadU16(),
            EndGlyphId = stream.ReadU16(),
            Class = stream.ReadU16(),
        };
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}