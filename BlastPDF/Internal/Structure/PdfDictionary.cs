using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfDictionary : PdfObject
{
    public IDictionary<PdfObject, PdfObject> Dictionary { get; set; }

    public PdfDictionary(IDictionary<PdfObject, PdfObject> dictionary) : base(PdfObjectType.DICTIONARY)
    {
        Dictionary = dictionary;
    }
}