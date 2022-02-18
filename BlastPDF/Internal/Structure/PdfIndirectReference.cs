namespace BlastPDF.Internal.Structure;

public class PdfIndirectReference : PdfObject
{
    public PdfNumeric ObjectNumber { get; set; }
    public PdfNumeric GenerationNumber { get; set; }

    public PdfIndirectReference(PdfNumeric objNum, PdfNumeric genNum) : base(PdfObjectType.INDIRECT_REF)
    {
        ObjectNumber = objNum;
        GenerationNumber = genNum;
    }
}