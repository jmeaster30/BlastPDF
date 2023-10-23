using BlastType.Internal.GlyphPositioningSubtables;
using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class GlyphPositioningTable : IFontTable
{
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public ushort ScriptListOffset { get; set; }
    public ushort FeatureListOffset { get; set; }
    public ushort LookupListOffset { get; set; }
    public ushort FeatureVariationsOffset { get; set; }

    public ScriptListTable? ScriptListTable { get; set; }
    public FeatureListTable? FeatureListTable { get; set; }
    public LookupListTable? LookupListTable { get; set; }
    public FeatureVariationsTable? FeatureVariationsTable { get; set; }

    public static GlyphPositioningTable Load(Stream stream)
    {
        var startOfTable = stream.Position;
        var gpos = new GlyphPositioningTable
        {
            MajorVersion = stream.ReadU16(),
            MinorVersion = stream.ReadU16(),
            ScriptListOffset = stream.ReadU16(),
            FeatureListOffset = stream.ReadU16(),
            LookupListOffset = stream.ReadU16(),
        };

        if (gpos.MinorVersion == 1)
        {
            gpos.FeatureVariationsOffset = stream.ReadU16();
        }
        
        Console.WriteLine(JsonConvert.SerializeObject(gpos));

        if (gpos.ScriptListOffset != 0)
        {
            stream.Seek(startOfTable + gpos.ScriptListOffset, SeekOrigin.Begin);
            gpos.ScriptListTable = ScriptListTable.Load(stream);
        }
        
        if (gpos.FeatureListOffset != 0)
        {
            stream.Seek(startOfTable + gpos.FeatureListOffset, SeekOrigin.Begin);
            //gpos.FeatureListTable = FeatureListTable.Load(stream);
        }
        
        if (gpos.LookupListOffset != 0)
        {
            stream.Seek(startOfTable + gpos.LookupListOffset, SeekOrigin.Begin);
            //gpos.LookupListTable = LookupListTable.Load(stream);
        }
        
        if (gpos.FeatureVariationsOffset != 0)
        {
            stream.Seek(startOfTable + gpos.FeatureVariationsOffset, SeekOrigin.Begin);
            //gpos.FeatureVariationsTable = FeatureVariationsTable.Load(stream);
        }
        
        return gpos;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(GlyphPositioningTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}