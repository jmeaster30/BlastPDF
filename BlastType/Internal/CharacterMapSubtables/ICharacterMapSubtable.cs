using MyLib.Streams;

namespace BlastType.Internal.CharacterMapSubtables;

public interface ICharacterMapSubtable
{
    public bool Is<T>();
    public string? ToString();

    public static ICharacterMapSubtable Load(Stream stream)
    {
        var format = stream.ReadU16();
        stream.Seek(-2, SeekOrigin.Current); // un read the 2 bytes

        if (format is not (0 or 2 or 4 or 6 or 8 or 10 or 12 or 13 or 14))
            throw new ArgumentException("Unexpected format of character map subtable");
        
        return format switch
        {
            0 => ByteEncodingTable.Load(stream),
            2 => HighByteMappingTable.Load(stream),
            4 => SegmentMapTable.Load(stream),
            6 => TrimmedMappingTable.Load(stream),
            8 => MixedCoverageTable.Load(stream),
            10 => TrimmedArrayTable.Load(stream),
            12 => SegmentedCoverageTable.Load(stream),
            13 => ManyToOneRangeMappingTable.Load(stream),
            14 => UnicodeVariationSequencesTable.Load(stream)
        };
    }
}