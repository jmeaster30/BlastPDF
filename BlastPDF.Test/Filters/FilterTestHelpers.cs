using System.Text;
using BlastPDF.Filter;
using BlastSharp.Lists;
using Xunit;

namespace BlastPDF.Test.Filters;

public class FilterTestHelpers
{
    // The idea is if the encoders and decoders are working properly then:
    // value == decode(encode(value))
    public static void TestFilter(PdfFilter filter, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        var encoded  = filter.Encode(bytes);
        var decoded = filter.Decode(encoded);
        var (diffOffset, leftDiff, rightDiff) = bytes.FirstDifference(decoded);
        Assert.True(diffOffset == -1, $"The encoder + decoder didn't produce the expected output at byte offset {diffOffset}. {leftDiff} != {rightDiff}");
    }
}