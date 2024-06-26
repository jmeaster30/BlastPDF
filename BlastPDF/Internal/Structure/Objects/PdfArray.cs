using System;
using System.Collections.Generic;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfArray : PdfObject
{
    public IEnumerable<PdfObject> Elements { get; set; }

    public PdfArray(IEnumerable<PdfObject> elements) : base(PdfObjectType.ARRAY)
    {
        Elements = elements;
    }

    public override void Print()
    {
        Console.WriteLine("[");
        foreach (var element in Elements)
        {
            element.Print();
            Console.WriteLine("");
        }
        Console.Write("]");
    }
}