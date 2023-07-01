using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 13
public class ManyToOneRangeMappingTable : ICharacterMapSubtable
{
    public static ManyToOneRangeMappingTable Load(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(ManyToOneRangeMappingTable);
    }
    
    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}