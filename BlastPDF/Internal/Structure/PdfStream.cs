using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfStream : PdfObject
{
    public PdfDictionary Dictionary { get; set; }
    
    // TODO change to a stream content object
    public Token Stream { get; set; }

    public PdfStream(PdfDictionary dictionary, Token stream) : base(PdfNodeType.STREAM)
    {
        Dictionary = dictionary;
        Stream = stream;
    }
}