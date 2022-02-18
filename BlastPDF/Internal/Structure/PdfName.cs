using System.Collections.Generic;

namespace BlastPDF.Internal.Structure;

public class PdfName : PdfObject
{
    public string Name { get; set; }
    public string ResolvedName { get; set; }
    
    public PdfName(string name) : base(PdfObjectType.NAME)
    {
        Name = name;
        ResolvedName = Name.ResolveName();
    }
}