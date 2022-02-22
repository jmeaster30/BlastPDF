using System.Collections.Generic;
using BlastPDF.Internal.Structure.Objects;

namespace BlastPDF.Internal.Structure.File;

public class PdfXReferenceTable
{
    public IEnumerable<PdfXReferenceSection> Sections { get; set; }
}

public class PdfXReferenceSection
{
    public IEnumerable<PdfXReferenceSubSection> SubSections { get; set; }
}

public class PdfXReferenceSubSection
{
    public PdfNumeric FirstObjectNumber { get; set; }
    public PdfNumeric NumberOfEntries { get; set; } // is this needed ??
    public IEnumerable<PdfXReferenceEntry> Entries { get; set; }
}


/*
 * ByteOffset - 10 byte string padded left with zeros
 * GenerationNumber - 5 byte string padded left with zeros
 * InUse - 'n' if true otherwise 'f'
 * entry line ends in two character end of line sequence, SP CR, SP LF, or CR LF
 * So, the total length of the entry is always 20 bytes long
 */
public class PdfXReferenceEntry
{
    public PdfNumeric ByteOffset { get; set; }
    public PdfNumeric GenerationNumber { get; set; }
    public bool InUse { get; set; }
}