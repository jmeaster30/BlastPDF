using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 14
public class UnicodeVariationSequencesTable : ICharacterMapSubtable
{
    public static UnicodeVariationSequencesTable Load(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(UnicodeVariationSequencesTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}