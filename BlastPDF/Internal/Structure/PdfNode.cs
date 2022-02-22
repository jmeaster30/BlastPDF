using System;

namespace BlastPDF.Internal.Structure;

public class PdfNode
{
    public PdfNodeType NodeType { get; set; }

    public PdfNode(PdfNodeType nodeType)
    {
        NodeType = nodeType;
    }
    
    public virtual void Print()
    {
        throw new NotImplementedException($"{NodeType} print not implemented.");
    }
}