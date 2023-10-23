using BlastType.Internal.DataTypes;
using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class PostTable : IFontTable
{
    public Fixed Version { get; set; } = default!;
    public Fixed ItalicAngle { get; set; } = default!;
    public short UnderlinePosition { get; set; }
    public short UnderlineThickness { get; set; }
    public uint IsFixedPitch { get; set; }
    public uint MinMemType42 { get; set; }
    public uint MaxMemType42 { get; set; }
    public uint MinMemType1 { get; set; }
    public uint MaxMemType1 { get; set; }

    public static PostTable Load(Stream stream)
    {
        var postTable = new PostTable
        {
            Version = Fixed.FromBytes(16, 16, stream.ReadBytes(4)),
            ItalicAngle = Fixed.FromBytes(16, 16, stream.ReadBytes(4)),
            UnderlinePosition = stream.ReadS16(),
            UnderlineThickness = stream.ReadS16(),
            IsFixedPitch = stream.ReadU32(),
            MinMemType42 = stream.ReadU32(),
            MaxMemType42 = stream.ReadU32(),
            MinMemType1 = stream.ReadU32(),
            MaxMemType1 = stream.ReadU32()
        };

        // TODO there are additional fields for each version
        
        return postTable;
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(PostTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}