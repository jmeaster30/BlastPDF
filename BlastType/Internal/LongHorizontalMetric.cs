using MyLib.Streams;

namespace BlastType.Internal;

public class LongHorizontalMetric
{
    public ushort AdvanceWidth { get; set; }
    public short LeftSideBearing { get; set; }

    public static LongHorizontalMetric Load(Stream stream)
    {
        return new LongHorizontalMetric
        {
            AdvanceWidth = stream.ReadU16(),
            LeftSideBearing = stream.ReadS16()
        };
    }
}