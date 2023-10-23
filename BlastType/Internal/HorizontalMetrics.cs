using MyLib.Streams;
using Newtonsoft.Json;

namespace BlastType.Internal;

public class HorizontalMetrics : IFontTable
{
    public List<LongHorizontalMetric> LongHorizontalMetrics { get; set; } = new();
    public List<short> LeftSideBearings { get; set; } = new();

    public static HorizontalMetrics Load(Stream stream, IEnumerable<IFontTable> fontTables)
    {
        // get the hhea table
        if (fontTables.FirstOrDefault(x => x.Is<HorizontalHeader>()) is not HorizontalHeader horizontalHeader) 
            throw new ArgumentException("Missing hhea table prior to the hmtx table :(", nameof(fontTables));
        
        // get the maxp table
        if (fontTables.FirstOrDefault(x => x.Is<MaximumProfile>()) is not MaximumProfile maximumProfile) 
            throw new ArgumentException("Missing hhea table prior to the hmtx table :(", nameof(fontTables));


        var metrics = new HorizontalMetrics();

        for (var i = 0; i < horizontalHeader.NumberOfHorizontalMetrics; i++)
        {
            metrics.LongHorizontalMetrics.Add(LongHorizontalMetric.Load(stream));
        }

        if (horizontalHeader.NumberOfHorizontalMetrics >= maximumProfile.NumberOfGlyphs) return metrics;
        
        for (var i = 0; i < maximumProfile.NumberOfGlyphs - horizontalHeader.NumberOfHorizontalMetrics; i++)
        {
            metrics.LeftSideBearings.Add(stream.ReadS16());
        }

        return metrics;
    }
    
    public bool Is<T>()
    {
        return typeof(T) == typeof(HorizontalMetrics);
    }

    public new string? ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}