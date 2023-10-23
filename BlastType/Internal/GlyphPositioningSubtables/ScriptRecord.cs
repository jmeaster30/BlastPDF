using MyLib.Streams;

namespace BlastType.Internal.GlyphPositioningSubtables;

public class ScriptRecord
{
    public byte[] ScriptTag { get; set; } = Array.Empty<byte>();
    public ushort ScriptOffset { get; set; }

    public static ScriptRecord Load(Stream stream)
    {
        return new ScriptRecord
        {
            ScriptTag = stream.ReadBytes(4),
            ScriptOffset = stream.ReadU16(),
        };
    }
}