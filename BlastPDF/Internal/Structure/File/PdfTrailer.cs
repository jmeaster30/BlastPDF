using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal.Structure.File;

public class PdfTrailer
{
    public PdfDictionary Dictionary { get; set; }
    public PdfNumeric LastCrossReferenceByteOffset { get; set; }
}