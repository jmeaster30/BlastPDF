using System.Collections.Generic;
using System.Linq;

namespace BlastPDF.Internal.Structure;

public class PdfComment : PdfObject
{
    public string Comment { get; set; }

    public PdfComment(string comment) : base(PdfObjectType.COMMENT)
    {
        Comment = $"%{comment}";
    }

    public PdfComment(IEnumerable<Token> comment) : base(PdfObjectType.COMMENT)
    {
        Comment = string.Join("", comment.Select(x => x.Lexeme));
    }
}