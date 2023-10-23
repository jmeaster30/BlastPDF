using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class Os2 : IFontTable
{
    public ushort Version { get; set; }
    public short XAvgCharWidth { get; set; }
    public ushort UsWeightClass { get; set; }
    public ushort UsWidthClass { get; set; }
    public ushort FsType { get; set; }
    public short YSubscriptXSize { get; set; }
    public short YSubscriptYSize { get; set; }
    public short YSubscriptXOffset { get; set; }
    public short YSubscriptYOffset { get; set; }
    public short YSuperscriptXSize { get; set; }
    public short YSuperscriptYSize { get; set; }
    public short YSuperscriptXOffset { get; set; }
    public short YSuperscriptYOffset { get; set; }
    public short YStrikeoutSize { get; set; }
    public short YStrikeoutPosition { get; set; }
    public short SFamilyClass { get; set; }
    public byte[] Panose { get; set; } = Array.Empty<byte>();
    public uint UlUnicodeRange1 { get; set; }
    public uint UlUnicodeRange2 { get; set; }
    public uint UlUnicodeRange3 { get; set; }
    public uint UlUnicodeRange4 { get; set; }
    public byte[] AchVendId { get; set; } = Array.Empty<byte>();
    public ushort FsSelection { get; set; }
    public ushort UsFirstCharIndex { get; set; }
    public ushort UsLastCharIndex { get; set; }
    public short STypoAscender { get; set; }
    public short STypoDescender { get; set; }
    public short STypoLineGap { get; set; }
    public ushort UsWinAscent { get; set; }
    public ushort UsWinDescent { get; set; }
    public uint UlCodePageRange1 { get; set; }
    public uint UlCodePageRange2 { get; set; }
    public short SxHeight { get; set; }
    public short SCapHeight { get; set; }
    public ushort UsDefaultChar { get; set; }
    public ushort UsBreakChar { get; set; }
    public ushort UsMaxContext { get; set; }

    public static Os2 Load(Stream stream)
    {
        var version = stream.ReadU16();
        switch (version) {
            case 0:
                return new Os2
                {
                    Version = version,
                    XAvgCharWidth = stream.ReadS16(),
                    UsWeightClass = stream.ReadU16(),
                    UsWidthClass = stream.ReadU16(),
                    FsType = stream.ReadU16(),
                    YSubscriptXSize = stream.ReadS16(),
                    YSubscriptYSize = stream.ReadS16(),
                    YSubscriptXOffset = stream.ReadS16(),
                    YSubscriptYOffset = stream.ReadS16(),
                    YSuperscriptXSize = stream.ReadS16(),
                    YSuperscriptYSize = stream.ReadS16(),
                    YSuperscriptXOffset = stream.ReadS16(),
                    YSuperscriptYOffset = stream.ReadS16(),
                    YStrikeoutSize = stream.ReadS16(),
                    YStrikeoutPosition = stream.ReadS16(),
                    SFamilyClass = stream.ReadS16(),
                    Panose = stream.ReadBytes(10),
                    UlUnicodeRange1 = stream.ReadU32(),
                    UlUnicodeRange2 = stream.ReadU32(),
                    UlUnicodeRange3 = stream.ReadU32(),
                    UlUnicodeRange4 = stream.ReadU32(),
                    AchVendId = stream.ReadBytes(4),
                    FsSelection = stream.ReadU16(),
                    UsFirstCharIndex = stream.ReadU16(),
                    UsLastCharIndex = stream.ReadU16(),
                    STypoAscender = stream.ReadS16(),
                    STypoDescender = stream.ReadS16(),
                    STypoLineGap = stream.ReadS16(),
                    UsWinAscent = stream.ReadU16(),
                    UsWinDescent = stream.ReadU16(),
                };
            case 1:
                return new Os2
                {
                    Version = version,
                    XAvgCharWidth = stream.ReadS16(),
                    UsWeightClass = stream.ReadU16(),
                    UsWidthClass = stream.ReadU16(),
                    FsType = stream.ReadU16(),
                    YSubscriptXSize = stream.ReadS16(),
                    YSubscriptYSize = stream.ReadS16(),
                    YSubscriptXOffset = stream.ReadS16(),
                    YSubscriptYOffset = stream.ReadS16(),
                    YSuperscriptXSize = stream.ReadS16(),
                    YSuperscriptYSize = stream.ReadS16(),
                    YSuperscriptXOffset = stream.ReadS16(),
                    YSuperscriptYOffset = stream.ReadS16(),
                    YStrikeoutSize = stream.ReadS16(),
                    YStrikeoutPosition = stream.ReadS16(),
                    SFamilyClass = stream.ReadS16(),
                    Panose = stream.ReadBytes(10),
                    UlUnicodeRange1 = stream.ReadU32(),
                    UlUnicodeRange2 = stream.ReadU32(),
                    UlUnicodeRange3 = stream.ReadU32(),
                    UlUnicodeRange4 = stream.ReadU32(),
                    AchVendId = stream.ReadBytes(4),
                    FsSelection = stream.ReadU16(),
                    UsFirstCharIndex = stream.ReadU16(),
                    UsLastCharIndex = stream.ReadU16(),
                    STypoAscender = stream.ReadS16(),
                    STypoDescender = stream.ReadS16(),
                    STypoLineGap = stream.ReadS16(),
                    UsWinAscent = stream.ReadU16(),
                    UsWinDescent = stream.ReadU16(),
                    UlCodePageRange1 = stream.ReadU32(),
                    UlCodePageRange2 = stream.ReadU32(),
                };
            case 3:
            case 4:
            case 5:
                return new Os2
                {
                    Version = version,
                    XAvgCharWidth = stream.ReadS16(),
                    UsWeightClass = stream.ReadU16(),
                    UsWidthClass = stream.ReadU16(),
                    FsType = stream.ReadU16(),
                    YSubscriptXSize = stream.ReadS16(),
                    YSubscriptYSize = stream.ReadS16(),
                    YSubscriptXOffset = stream.ReadS16(),
                    YSubscriptYOffset = stream.ReadS16(),
                    YSuperscriptXSize  = stream.ReadS16(),
                    YSuperscriptYSize = stream.ReadS16(),
                    YSuperscriptXOffset = stream.ReadS16(),
                    YSuperscriptYOffset = stream.ReadS16(),
                    YStrikeoutSize = stream.ReadS16(),
                    YStrikeoutPosition = stream.ReadS16(),
                    SFamilyClass = stream.ReadS16(),
                    Panose = stream.ReadBytes(10),
                    UlUnicodeRange1 = stream.ReadU32(),
                    UlUnicodeRange2 = stream.ReadU32(),
                    UlUnicodeRange3 = stream.ReadU32(),
                    UlUnicodeRange4 = stream.ReadU32(),
                    AchVendId = stream.ReadBytes(4),
                    FsSelection = stream.ReadU16(),
                    UsFirstCharIndex = stream.ReadU16(),
                    UsLastCharIndex = stream.ReadU16(),
                    STypoAscender = stream.ReadS16(),
                    STypoDescender = stream.ReadS16(),
                    STypoLineGap = stream.ReadS16(),
                    UsWinAscent = stream.ReadU16(),
                    UsWinDescent = stream.ReadU16(),
                    UlCodePageRange1 = stream.ReadU32(),
                    UlCodePageRange2 = stream.ReadU32(),
                    SxHeight = stream.ReadS16(),
                    SCapHeight = stream.ReadS16(),
                    UsDefaultChar = stream.ReadU16(),
                    UsBreakChar = stream.ReadU16(),
                    UsMaxContext = stream.ReadU16()
                };
            default:
                throw new ArgumentException($"Unexpected OS/2 version number {version}");
        }

        
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(Os2);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}