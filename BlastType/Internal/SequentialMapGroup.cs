using MyLib.Streams;

namespace BlastType.Internal;

public class SequentialMapGroup
{
    public uint StartCharacterCode { get; set; }
    public uint EndCharacterCode { get; set; }
    public uint StartGlyphId { get; set; }

    public static SequentialMapGroup Load(Stream stream)
    {
        return new SequentialMapGroup
        {
            StartCharacterCode = stream.ReadU32(),
            EndCharacterCode = stream.ReadU32(),
            StartGlyphId = stream.ReadU32(),
        };
    }
}