using System;

namespace BlastPDF.Internal.Structure;

public class PdfObject
{
    public PdfObjectType ObjectType { get; set; }

    public PdfObject(PdfObjectType objectType)
    {
        ObjectType = objectType;
    }
    
    public virtual void Print()
    {
        Console.WriteLine($"WHOOPS :: {ObjectType} print not implemented.");
    }
}