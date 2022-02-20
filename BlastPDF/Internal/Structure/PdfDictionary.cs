using System;
using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfDictionary : PdfObject
{
    public IDictionary<PdfName, PdfObject> Dictionary { get; set; }

    public PdfDictionary(IDictionary<PdfName, PdfObject> dictionary) : base(PdfObjectType.DICTIONARY)
    {
        Dictionary = dictionary;
    }

    public override void Print()
    {
        Console.WriteLine("DICTIONARY START");
        foreach (var keyvalue in Dictionary)
        {
            Console.WriteLine("PAIR START");
            keyvalue.Key.Print();
            keyvalue.Value.Print();
            Console.WriteLine("PAIR END");
        }
        Console.WriteLine("DICTIONARY END");
    }
}