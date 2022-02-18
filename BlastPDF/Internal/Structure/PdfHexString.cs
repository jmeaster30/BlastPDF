namespace BlastPDF.Internal.Structure;

public class PdfHexString : PdfObject
{
    public string StringValue { get; set; }
    public string ResolvedValue { get; set; }

    public PdfHexString(string value) : base(PdfObjectType.HEX_STRING)
    {
        StringValue = value;
        ResolvedValue = StringValue.ResolveHexString();
    }
}