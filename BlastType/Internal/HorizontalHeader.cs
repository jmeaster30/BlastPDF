using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class HorizontalHeader : IFontTable
{
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public short Ascender { get; set; }
    public short Descender { get; set; }
    public short LineGap { get; set; }
    public ushort AdvanceWidthMax { get; set; }
    public short MinLeftSideBearing { get; set; }
    public short MinRightSideBearing { get; set; }
    public short XMaxExtent { get; set; }
    public short CaretSlopeRise { get; set; }
    public short CaretSlopeRun { get; set; }
    public short CaretOffset { get; set; }
    public short Reserved1 { get; set; }
    public short Reserved2 { get; set; }
    public short Reserved3 { get; set; }
    public short Reserved4 { get; set; }
    public short MetricDataFormat { get; set; }
    public ushort NumberOfHorizontalMetrics { get; set; }

    public bool Is<T>()
    {
        return typeof(T) == typeof(HorizontalHeader);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static HorizontalHeader Load(Stream stream)
    {
        return new HorizontalHeader
        {
            MajorVersion = stream.ReadU16(),
            MinorVersion = stream.ReadU16(),
            Ascender = stream.ReadS16(),
            Descender = stream.ReadS16(),
            LineGap = stream.ReadS16(),
            AdvanceWidthMax = stream.ReadU16(),
            MinLeftSideBearing = stream.ReadS16(),
            MinRightSideBearing = stream.ReadS16(),
            XMaxExtent = stream.ReadS16(),
            CaretSlopeRise = stream.ReadS16(),
            CaretSlopeRun = stream.ReadS16(),
            CaretOffset = stream.ReadS16(),
            Reserved1 = stream.ReadS16(),
            Reserved2 = stream.ReadS16(),
            Reserved3 = stream.ReadS16(),
            Reserved4 = stream.ReadS16(),
            MetricDataFormat = stream.ReadS16(),
            NumberOfHorizontalMetrics = stream.ReadU16(),
        };
    }
}