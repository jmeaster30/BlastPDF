using System.Collections.Generic;

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
}