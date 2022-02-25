using System;
using System.Collections.Generic;
using System.IO;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfStream : PdfObject
{
    public PdfDictionary Dictionary { get; set; }
    
    public PdfStreamContent Stream { get; set; }

    public PdfStream(PdfDictionary dictionary, Token stream) : base(PdfObjectType.STREAM)
    {
        Dictionary = dictionary;
        Stream = new(stream);
    }
    
    public override void Print()
    {
        Dictionary.Print();
        Console.Write("\nstream\n");
        Stream.Print();
        Console.Write("\nendstream\n");
    }
}