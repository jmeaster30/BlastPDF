using System.Collections.Generic;

namespace BlastPDF.Internal.Structure.Objects;

public class PdfXReferenceTable : PdfObject
{
    public IEnumerable<PdfXReferenceSection> Sections { get; set; }

    public PdfXReferenceTable(IEnumerable<PdfXReferenceSection> sections) : base(PdfObjectType.XREF_TABLE)
    {
        Sections = sections;
    }
}

public class PdfXReferenceSection : PdfObject
{
    public IEnumerable<PdfXReferenceSubSection> SubSections { get; set; }

    public PdfXReferenceSection(IEnumerable<PdfXReferenceSubSection> subsections) : base(PdfObjectType.XREF_SECTION)
    {
        SubSections = subsections;
    }
}

public class PdfXReferenceSubSection : PdfObject
{
    public PdfNumeric FirstObjectNumber { get; set; }
    public PdfNumeric NumberOfEntries { get; set; }
    public IEnumerable<PdfXReferenceEntry> Entries { get; set; }
    
    public PdfXReferenceSubSection(
        PdfNumeric objNumber,
        PdfNumeric numOfEntries,
        IEnumerable<PdfXReferenceEntry> entries)
        : base(PdfObjectType.XREF_SUB_SECTION)
    {
        FirstObjectNumber = objNumber;
        NumberOfEntries = numOfEntries;
        Entries = entries;
    }
}


/*
 * ByteOffset - 10 byte string padded left with zeros
 * GenerationNumber - 5 byte string padded left with zeros
 * InUse - 'n' if true otherwise 'f'
 * entry line ends in two character end of line sequence, SP CR, SP LF, or CR LF
 * So, the total length of the entry is always 20 bytes long
 */
public class PdfXReferenceEntry : PdfObject
{
    public PdfNumeric ByteOffset { get; set; }
    public PdfNumeric GenerationNumber { get; set; }
    public bool InUse { get; set; }
    
    public PdfXReferenceEntry(
        PdfNumeric byteOffset,
        PdfNumeric genNumber,
        bool inUse)
        : base(PdfObjectType.XREF_ENTRY)
    {
        ByteOffset = byteOffset;
        GenerationNumber = genNumber;
        InUse = inUse;
    }
}