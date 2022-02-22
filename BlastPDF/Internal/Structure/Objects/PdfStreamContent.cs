using System.Collections.Generic;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfStreamContent
{
    public Token Token { get; set; }
    public IEnumerable<PdfObject> Objects { get; set; }
}