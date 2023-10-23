using BlastType.Internal.GlyphDefinitionSubtables;
using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class GlyphDefinitionTable : IFontTable
{
    // Header
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public ushort GlyphClassDefOffset { get; set; }
    public ushort AttachListOffset { get; set; }
    public ushort LigatureCaretListOffset { get; set; }
    public ushort MarkAttachClassDefOffset { get; set; }
    public ushort MarkGlyphSetsDefOffset { get; set; }
    public ushort ItemVarStoreOffset { get; set; }

    public GlyphClassDefinitionTable? GlyphClassDefinitionTable { get; set; }
    public AttachmentPointListTable? AttachmentPointListTable { get; set; }
    public LigatureCaretListTable? LigatureCaretListTable { get; set; }
    public MarkAttachClassDefinitionTable? MarkAttachClassDefinitionTable { get; set; }
    public MarkGlyphSetsDefinitionTable? MarkGlyphSetsDefinitionTable { get; set; }
    public ItemVarStoreTable? ItemVarStoreTable { get; set; }

    public static GlyphDefinitionTable Load(Stream stream)
    {
        var startOfTable = stream.Position;
        var gdef = new GlyphDefinitionTable
        {
            MajorVersion = stream.ReadU16(),
            MinorVersion = stream.ReadU16(),
            GlyphClassDefOffset = stream.ReadU16(),
            AttachListOffset = stream.ReadU16(),
            LigatureCaretListOffset = stream.ReadU16(),
            MarkAttachClassDefOffset = stream.ReadU16()
        };
        
        if (gdef.MinorVersion is 2 or 3)
        {
            gdef.MarkGlyphSetsDefOffset = stream.ReadU16();
        }
        
        if (gdef.MinorVersion is 3)
        {
            gdef.ItemVarStoreOffset = stream.ReadU16();
        }

        if (gdef.GlyphClassDefOffset != 0)
        {
            stream.Seek(startOfTable + gdef.GlyphClassDefOffset, SeekOrigin.Begin);
            gdef.GlyphClassDefinitionTable = GlyphClassDefinitionTable.Load(stream);
        }

        if (gdef.AttachListOffset != 0)
        {
            stream.Seek(startOfTable + gdef.AttachListOffset, SeekOrigin.Begin);
            gdef.AttachmentPointListTable = AttachmentPointListTable.Load(stream);
        }
        
        if (gdef.LigatureCaretListOffset != 0)
        {
            stream.Seek(startOfTable + gdef.LigatureCaretListOffset, SeekOrigin.Begin);
            gdef.LigatureCaretListTable = LigatureCaretListTable.Load(stream);
        }
        
        if (gdef.MarkAttachClassDefOffset != 0)
        {
            stream.Seek(startOfTable + gdef.MarkAttachClassDefOffset, SeekOrigin.Begin);
            gdef.MarkAttachClassDefinitionTable = MarkAttachClassDefinitionTable.Load(stream);
        }
        
        if (gdef.MarkGlyphSetsDefOffset != 0)
        {
            stream.Seek(startOfTable + gdef.MarkGlyphSetsDefOffset, SeekOrigin.Begin);
            gdef.MarkGlyphSetsDefinitionTable = MarkGlyphSetsDefinitionTable.Load(stream);
        }

        if (gdef.ItemVarStoreOffset != 0)
        {
            stream.Seek(startOfTable + gdef.ItemVarStoreOffset, SeekOrigin.Begin);
            gdef.ItemVarStoreTable = ItemVarStoreTable.Load(stream);
        }

        return gdef;
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(GlyphDefinitionTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}

