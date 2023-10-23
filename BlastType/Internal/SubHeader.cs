using MyLib.Streams;

namespace BlastType.Internal;

public class SubHeader
{
    public ushort FirstCode { get; set; }
    public ushort EntryCount { get; set; }
    public short IdDelta { get; set; }
    public ushort IdRangeOffset { get; set; }

    public static SubHeader Load(Stream stream)
    {
        return new SubHeader
        {
            FirstCode = stream.ReadU16(),
            EntryCount = stream.ReadU16(),
            IdDelta = stream.ReadS16(),
            IdRangeOffset = stream.ReadU16()
        };
    }
}