using Newtonsoft.Json;

namespace BlastType.Internal.CharacterMapSubtables;

// cmap format 12
public class SegmentedCoverageTable : ICharacterMapSubtable
{
    public static SegmentedCoverageTable Load(Stream stream)
    {
        throw new NotImplementedException();
    }

    public bool Is<T>()
    {
        return typeof(T) == typeof(SegmentedCoverageTable);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}