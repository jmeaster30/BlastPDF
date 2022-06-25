using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfStartXref : PdfObject
{
  public PdfNumeric Offset { get; }

  public PdfStartXref(PdfNumeric offset) : base(PdfObjectType.START_XREF)
  {
    Offset = offset;
  }

  public override void Print()
  {
    Console.WriteLine("startxref");
    Offset.Print();
    Console.WriteLine("");
  }

}