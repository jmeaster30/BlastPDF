using BlastType.Internal.CompactFontFormatSubtables;

namespace BlastType.Internal;

public class CompactFontFormatTable : IFontTable
{
    public CompactFontFormatHeader Header { get; set; }

    public static CompactFontFormatTable Load(Stream stream)
    {
        CompactFontFormatHeader header = CompactFontFormatHeader.Load(stream);

        return new CompactFontFormatTable
        {
            Header = header
        };
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(CompactFontFormatTable);
    }
}