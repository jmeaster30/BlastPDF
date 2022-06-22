using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfIndirectReference : PdfObject
{
    public PdfNumeric ObjectNumber { get; set; }
    public PdfNumeric GenerationNumber { get; set; }

    public PdfIndirectReference(PdfNumeric objNum, PdfNumeric genNum) : base(PdfObjectType.INDIRECT_REF)
    {
        ObjectNumber = objNum;
        GenerationNumber = genNum;
    }

    public override void Print()
    {
        ObjectNumber.Print();
        Console.Write(" ");
        GenerationNumber.Print();
        Console.Write(" R");
    }
}