namespace BlastPDF.Internal.Structure;

public class PdfIndirectObject : PdfObject
{
    public PdfNumeric ObjectNumber { get; set; }
    public PdfNumeric GenerationNumber { get; set; }
    public PdfObject Value { get; set; }

    public PdfIndirectObject(PdfNumeric objNum, PdfNumeric genNum, PdfObject value) : base(PdfNodeType.INDIRECT_OBJ)
    {
        ObjectNumber = objNum;
        GenerationNumber = genNum;
        Value = value;
    }
}