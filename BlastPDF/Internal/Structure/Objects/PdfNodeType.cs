namespace BlastPDF.Internal.Structure.Objects;

public enum PdfNodeType
{
    COMMENT, BOOLEAN, NUMERIC, NULL,
    LITERAL_STRING, HEX_STRING, NAME, 
    ARRAY, DICTIONARY, STREAM,
    INDIRECT_OBJ, INDIRECT_REF,
}