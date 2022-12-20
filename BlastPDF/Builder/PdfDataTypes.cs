using System;
using System.Collections.Generic;
using System.Linq;
using BlastSharp.Compression;
using BlastSharp.Lists;

namespace BlastPDF.Builder;

public enum PdfFilter
{
    ASCIIHexDecode,
    ASCII85Decode,
    LZWDecode,
    FlateDeode,
    RunLengthDecode,
    CCITTFaxDecode,
    JBIG2Decode,
    DCTDecode,
    JPXDecode,
    Crypt,
}

public static class PdfFilterExtensions
{
    

    private static ICompressionAlgorithm Get(this PdfFilter decodeType)
    {
        return decodeType switch
        {
            PdfFilter.ASCIIHexDecode => new AsciiHex(),
            PdfFilter.ASCII85Decode => new Ascii85(),
            PdfFilter.LZWDecode => throw new NotImplementedException(),
            PdfFilter.FlateDeode => throw new NotImplementedException(),
            PdfFilter.RunLengthDecode => new RunLength(),
            PdfFilter.CCITTFaxDecode => throw new NotImplementedException(),
            PdfFilter.JBIG2Decode => throw new NotImplementedException(),
            PdfFilter.DCTDecode => throw new NotImplementedException(),
            PdfFilter.JPXDecode => throw new NotImplementedException(),
            PdfFilter.Crypt => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(decodeType), decodeType, "Unsupported PDF Filter :(")
        };
    }

    public static IEnumerable<byte> Encode(this IEnumerable<byte> input, PdfFilter filter) =>
        filter.Get().Encode(input);
}