using System.Collections.Generic;
using System.Linq;

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
    
    public PdfName(IEnumerable<Token> name) : base(PdfObjectType.NAME)
    {
        Name = string.Join("", name.Select(x => x.Lexeme));
        ResolvedName = Name.ResolveName();
    }
}