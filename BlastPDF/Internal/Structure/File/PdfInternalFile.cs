using System.Collections.Generic;
using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal.Structure.File;

public class PdfInternalFile
{
    public PdfComment Header { get; set; }
    public PdfComment BinaryComment { get; set; }
    public List<PdfInternalBody> Body { get; set; } = new List<PdfInternalBody>();
}

public class PdfInternalBody
{
    public List<PdfObject> Contents { get; set; }
    public PdfXReferenceTable CrossReferenceTable { get; set; }
    public PdfTrailer Trailer { get; set; }
    public PdfComment Footer { get; set; }
}
