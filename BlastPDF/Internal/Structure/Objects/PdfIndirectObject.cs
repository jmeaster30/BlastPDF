using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfIndirectObject : PdfObject
{
    public PdfNumeric ObjectNumber { get; set; }
    public PdfNumeric GenerationNumber { get; set; }
    public PdfObject Value { get; set; }

    public PdfIndirectObject(PdfNumeric objNum, PdfNumeric genNum, PdfObject value) : base(PdfObjectType.INDIRECT_OBJ)
    {
        ObjectNumber = objNum;
        GenerationNumber = genNum;
        Value = value;
    }

    public override void Print()
    {
        ObjectNumber.Print();
        Console.Write(" ");
        GenerationNumber.Print();
        Console.WriteLine(" obj");
        Value.Print();
        Console.WriteLine("endobj");
    }
}