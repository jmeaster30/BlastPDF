using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfStream : PdfObject
{
    public PdfDictionary Dictionary { get; set; }
    
    public PdfStreamContent Stream { get; set; }

    public PdfStream(PdfDictionary dictionary, Token stream) : base(PdfNodeType.STREAM)
    {
        Dictionary = dictionary;
        Stream = new()
        {
            Token = stream
        };
    }
    
    public PdfStream(PdfDictionary dictionary, IEnumerable<PdfObject> stream) : base(PdfNodeType.STREAM)
    {
        Dictionary = dictionary;
        Stream = new()
        {
            Objects = stream
        };
    }
}