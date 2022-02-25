using System;
using System.Collections.Generic;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfStreamContent : PdfObject
{
    public Token Token { get; set; }
    public IEnumerable<PdfObject> Objects { get; set; }

    public PdfStreamContent(Token token) : base(PdfObjectType.STREAM_CONTENT)
    {
        Token = token;
        Objects = null;
    }

    public override void Print()
    {
        if (Objects == null)
            Console.Write(Token.Lexeme);
        else
            foreach (var obj in Objects)
                obj.Print();
    }
}