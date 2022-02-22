using System;
using System.Collections.Generic;
using System.Linq;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfComment : PdfObject
{
    public Token Comment { get; set; }

    public PdfComment(Token comment) : base(PdfNodeType.COMMENT)
    {
        Comment = comment;
    }

    public override void Print()
    {
        Console.WriteLine(Comment.Lexeme);
    }
}