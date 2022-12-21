using System.Collections.Generic;

namespace BlastPDF.Filter.Implementations;

public interface ICompressionAlgorithm
{
    public IEnumerable<byte> Encode(IEnumerable<byte> input);
    public IEnumerable<byte> Decode(IEnumerable<byte> input);
}