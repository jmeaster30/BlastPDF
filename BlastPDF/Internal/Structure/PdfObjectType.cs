namespace BlastPDF.Internal.Structure;

public enum PdfObjectType
{
    COMMENT, BOOLEAN, NUMERIC, NULL,
    LITERAL_STRING, HEX_STRING, NAME, 
    ARRAY, DICTIONARY, STREAM,
    INDIRECT_OBJ, INDIRECT_REF
}