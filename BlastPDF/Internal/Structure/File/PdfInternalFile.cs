using System.Collections.Generic;
using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal.Structure.File;

public class PdfInternalFile
{
    public PdfComment Header { get; set; }
    public IEnumerable<PdfObject> Body { get; set; }
    public PdfCrossReferenceTable CrossReferenceTable { get; set; }
    public PdfTrailer Trailer { get; set; }
}