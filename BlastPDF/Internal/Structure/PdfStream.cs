using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfStream : PdfObject
{
    public PdfDictionary Dictionary { get; set; }
    public IEnumerable<Token> Stream { get; set; }

    public PdfStream(PdfDictionary dictionary, IEnumerable<Token> stream) : base(PdfObjectType.STREAM)
    {
        Dictionary = dictionary;
        Stream = stream;
    }
}