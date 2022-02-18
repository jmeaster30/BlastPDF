using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfArray : PdfObject
{
    public IEnumerable<PdfObject> Elements { get; set; }

    public PdfArray(IEnumerable<PdfObject> elements) : base(PdfObjectType.ARRAY)
    {
        Elements = elements;
    }
}