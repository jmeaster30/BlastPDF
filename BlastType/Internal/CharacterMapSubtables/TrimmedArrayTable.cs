using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 10
public class TrimmedArrayTable : ICharacterMapSubtable
{
    public static TrimmedArrayTable Load(Stream stream)
    {
        throw new NotImplementedException();
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(TrimmedArrayTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}