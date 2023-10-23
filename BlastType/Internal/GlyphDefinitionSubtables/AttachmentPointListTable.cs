using MyLib.Streams;

namespace BlastType.Internal.GlyphDefinitionSubtables;

public class AttachmentPointListTable
{
    public ushort CoverageOffset { get; set; }
    public ushort GlyphCount { get; set; }
    public List<ushort> AttachPointOffsets { get; set; } = new();

    public static AttachmentPointListTable Load(Stream stream)
    {
        var apl = new AttachmentPointListTable
        {
            CoverageOffset = stream.ReadU16(),
            GlyphCount = stream.ReadU16(),
        };

        for (var i = 0; i < apl.GlyphCount; i++)
        {
            apl.AttachPointOffsets.Add(stream.ReadU16());
        }
        
        return apl;
    }
}