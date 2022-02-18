namespace BlastPDF.Internal.Structure;

public class PdfObject
{
    public PdfObjectType ObjectType { get; set; }

    public PdfObject(PdfObjectType objType)
    {
        ObjectType = objType;
    }
    
}