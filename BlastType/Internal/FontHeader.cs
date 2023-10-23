using BlastType.Internal.DataTypes;
using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class FontHeader : IFontTable
{
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public Fixed FontRevision { get; set; } = default!;
    public uint CheckSumAdjustment { get; set; }
    public uint MagicNumber { get; set; } // 5F0F3CF5
    public ushort Flags { get; set; }
    public ushort UnitsPerEm { get; set; }
    public LongDateTime Created { get; set; } = default!;
    public LongDateTime Modified { get; set; } = default!;
    public short XMin { get; set; }
    public short YMin { get; set; }
    public short XMax { get; set; }
    public short YMax { get; set; }
    public ushort MacStyle { get; set; }
    public ushort LowestRecPpem { get; set; }
    public short FontDirectionHint { get; set; }
    public short IndexToLocFormat { get; set; }
    public short GlyphDataFormat { get; set; }

    public bool Is<T>()
    {
        return typeof(T) == typeof(FontHeader);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    
    public static FontHeader Load(Stream stream)
    {
        return new FontHeader
        {
            MajorVersion = stream.ReadU16(),
            MinorVersion = stream.ReadU16(),
            FontRevision = Fixed.FromBytes(16, 16, stream.ReadBytes(4)),
            CheckSumAdjustment = stream.ReadU32(),
            MagicNumber = stream.ReadU32(),
            Flags = stream.ReadU16(),
            UnitsPerEm = stream.ReadU16(),
            Created = LongDateTime.FromBytes(stream.ReadBytes(8)),
            Modified = LongDateTime.FromBytes(stream.ReadBytes(8)),
            XMin = stream.ReadS16(),
            YMin = stream.ReadS16(),
            XMax = stream.ReadS16(),
            YMax = stream.ReadS16(),
            MacStyle = stream.ReadU16(),
            LowestRecPpem = stream.ReadU16(),
            FontDirectionHint = stream.ReadS16(),
            IndexToLocFormat = stream.ReadS16(),
            GlyphDataFormat = stream.ReadS16(),
        };
    }
}