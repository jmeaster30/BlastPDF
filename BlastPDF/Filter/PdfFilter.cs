using System;
using System.Collections.Generic;
using BlastPDF.Filter;
using BlastPDF.Filter.Implementations;

namespace BlastPDF.Builder;

public enum PdfFilter
{
    ASCIIHex,
    ASCII85,
    LZW,
    Flate,
    RunLength,
    CCITTFax,
    JBIG2,
    DCT,
    JPX,
    Crypt,
}

public static class PdfFilterExtensions
{
    private static IFilterAlgorithm Get(this PdfFilter decodeType, IFilterParameters parameters)
    {
        return decodeType switch
        {
            PdfFilter.ASCIIHex => new AsciiHex(),
            PdfFilter.ASCII85 => new Ascii85(),
            PdfFilter.LZW => new Lzw(parameters as LzwParameters),
            PdfFilter.Flate => throw new NotImplementedException(),
            PdfFilter.RunLength => new RunLength(),
            PdfFilter.CCITTFax => throw new NotImplementedException(),
            PdfFilter.JBIG2 => throw new NotImplementedException(),
            PdfFilter.DCT => throw new NotImplementedException(),
            PdfFilter.JPX => throw new NotImplementedException(),
            PdfFilter.Crypt => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(decodeType), decodeType, "Unsupported PDF Filter :(")
        };
    }

    public static IEnumerable<byte> Encode(this PdfFilter filter, IEnumerable<byte> input, IFilterParameters parameters = null) =>
        filter.Get(parameters).Encode(input);
    
    public static IEnumerable<byte> Decode(this PdfFilter filter, IEnumerable<byte> input, IFilterParameters parameters = null) =>
        filter.Get(parameters).Decode(input);
}