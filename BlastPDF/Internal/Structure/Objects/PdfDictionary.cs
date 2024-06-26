using System;
using System.Collections.Generic;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfDictionary : PdfObject
{
    public IDictionary<PdfName, PdfObject> Dictionary { get; set; }

    public PdfDictionary(IDictionary<PdfName, PdfObject> dictionary) : base(PdfObjectType.DICTIONARY)
    {
        Dictionary = dictionary;
    }

    public override void Print()
    {
        Console.WriteLine("<<");
        foreach (var (key, value) in Dictionary)
        {
            key.Print();
            Console.Write(" ");
            value.Print();
            Console.WriteLine("");
        }
        Console.Write(">>");
    }
}