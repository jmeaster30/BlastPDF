using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal.Structure.File;

public class PdfTrailer : PdfObject
{
    public PdfDictionary Dictionary { get; }
    public PdfNumeric LastCrossReferenceByteOffset { get; }

    public PdfTrailer(PdfDictionary dictionary, PdfNumeric byteOffset) : base(PdfObjectType.TRAILER)
    {
        Dictionary = dictionary;
        LastCrossReferenceByteOffset = byteOffset;
    }
}