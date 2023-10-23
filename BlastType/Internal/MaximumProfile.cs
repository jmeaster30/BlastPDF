using BlastType.Internal.DataTypes;
using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class MaximumProfile : IFontTable
{
    // Version 0.5 and 1 fields
    public Fixed Version { get; set; } = default!;
    public ushort NumberOfGlyphs { get; set; }
    
    // Version 1.0 fields
    public ushort? MaxPoints { get; set; }
    public ushort? MaxContours { get; set; }
    public ushort? MaxCompositePoints { get; set; }
    public ushort? MaxCompositeContours { get; set; }
    public ushort? MaxZones { get; set; }
    public ushort? MaxTwilightPoints { get; set; }
    public ushort? MaxStorage { get; set; }
    public ushort? MaxFunctionDefs { get; set; }
    public ushort? MaxInstructionDefs { get; set; }
    public ushort? MaxStackElements { get; set; }
    public ushort? MaxSizeOfInstructions { get; set; }
    public ushort? MaxComponentElements { get; set; }
    public ushort? MaxComponentDepth { get; set; }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(MaximumProfile);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static MaximumProfile Load(Stream stream)
    {
        var version = Fixed.FromBytes(16, 16, stream.ReadBytes(4));
        var numberOfGlyphs = stream.ReadU16();

        if (version.Value != 1)
        {
            return new MaximumProfile
            {
                Version = version,
                NumberOfGlyphs = numberOfGlyphs
            };
        }
        
        return new MaximumProfile
        {
            Version = version,
            NumberOfGlyphs = numberOfGlyphs,
            MaxPoints = stream.ReadU16(),
            MaxContours = stream.ReadU16(),
            MaxCompositePoints = stream.ReadU16(),
            MaxCompositeContours = stream.ReadU16(),
            MaxZones = stream.ReadU16(),
            MaxTwilightPoints = stream.ReadU16(),
            MaxStorage = stream.ReadU16(),
            MaxFunctionDefs = stream.ReadU16(),
            MaxInstructionDefs = stream.ReadU16(),
            MaxStackElements = stream.ReadU16(),
            MaxSizeOfInstructions = stream.ReadU16(),
            MaxComponentElements = stream.ReadU16(),
            MaxComponentDepth = stream.ReadU16(),
        };
    }
}