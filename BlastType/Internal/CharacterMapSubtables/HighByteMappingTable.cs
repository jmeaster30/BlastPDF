using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 2
public class HighByteMappingTable : ICharacterMapSubtable
{
    public ushort Format { get; set; }
    public ushort Length { get; set; }
    public ushort Language { get; set; }
    public byte[] SubHeaderKeys { get; set; }
    public List<SubHeader> SubHeaders { get; set; }
    public List<ushort> GlyphIndexArray { get; set; }

    public static HighByteMappingTable Load(Stream stream)
    {
        // How do we know the length of the subheaders????
        throw new NotImplementedException();
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(HighByteMappingTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}