using System;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfNode
{
    public PdfNodeType NodeType { get; set; }

    public PdfNode(PdfNodeType nodeType)
    {
        NodeType = nodeType;
    }
    
    public virtual void Print()
    {
        Console.WriteLine($"WHOOPS :: {NodeType} print not implemented.");
    }
}