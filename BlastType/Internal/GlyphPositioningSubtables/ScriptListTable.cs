using MyLib.Streams;

namespace BlastType.Internal.GlyphPositioningSubtables;

public class ScriptListTable
{
    public ushort ScriptCount { get; set; }
    public List<ScriptRecord> ScriptRecords { get; set; } = new();

    public static ScriptListTable Load(Stream stream)
    {
        var slt = new ScriptListTable
        {
            ScriptCount = stream.ReadU16()
        };

        for (var i = 0; i < slt.ScriptCount; i++)
        {
            slt.ScriptRecords.Add(ScriptRecord.Load(stream));
        }

        return slt;
    }
}