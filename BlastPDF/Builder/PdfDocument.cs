using System.IO;
using System.Collections.Generic;
using System.Linq;
using BlastPDF.Builder.Exporter;
using BlastPDF.Builder.Util;

namespace BlastPDF.Builder;

public class PdfDocument {
  public List<PdfPage> pages { get; set; } = new List<PdfPage>();

  public static PdfDocument Create() {
    return new PdfDocument();
  }

  public PdfDocument AddPage(PdfPage page) {
    page.SetDocument(this);
    pages.Add(page);
    return this;
  }

  public void Save(Stream stream) {
    Export(stream);
  }

  private PdfExporterResults Export(Stream stream, int startObjectNumber = 3) {
    stream.Write("%PDF-1.7\n".ToUTF8());
    stream.Write("%%EOF\n\n".ToUTF8());

    var crossReferences = new List<(int, long)>();
    var pageRefNumbers = new List<int>();

    var nextStart = startObjectNumber;
    foreach(var page in pages) {
      var refs = page.Export(stream, nextStart);
      crossReferences.AddRange(refs.ObjectNumberByteOffsets);
      if (refs.ObjectNumberByteOffsets.Any()) {
        nextStart = refs.ObjectNumberByteOffsets.Select(x => x.Item1).OrderBy(x => x).FirstOrDefault() + 1;
      }
      pageRefNumbers.AddRange(refs.PageRefNumber);
    }

    // body
    // document catalog
    // page tree
    // pages / objects

    crossReferences.Add((2, stream.Position));
    stream.Write($"2 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Pages\n".ToUTF8());
    stream.Write("/Kids [\n".ToUTF8());
    foreach(var pageRef in pageRefNumbers) {
      stream.Write($"{pageRef} 0 R\n".ToUTF8());
    }
    stream.Write("]\n".ToUTF8());
    stream.Write($"/Count {pageRefNumbers.Count}\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n".ToUTF8());

    crossReferences.Add((1, stream.Position));
    stream.Write($"1 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Catalog\n".ToUTF8());
    stream.Write($"/Pages 2 0 R\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n".ToUTF8());

    // cross reference

    var xrefByteOffset = stream.Position;
    stream.Write("xref\n".ToUTF8());
    stream.Write($"0 {crossReferences.Count + 1}\n".ToUTF8());

    crossReferences.Sort((a, b) => a.Item1 - b.Item1);
    stream.Write($"0000000000 65535 f\n".ToUTF8());
    foreach(var xref in crossReferences) {
      stream.Write($"{xref.Item2.ToString().PadLeft(10, '0')} 00000 n\n".ToUTF8());
    }
    stream.Write("trailer\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write($"/Size {crossReferences.Count + 1}\n".ToUTF8());
    stream.Write($"/Root 1 0 R\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("startxref\n".ToUTF8());
    stream.Write($"{xrefByteOffset}\n".ToUTF8());
    stream.Write("%%EOF\n".ToUTF8());

    // Trailer
    // need to add sizeof cross reference section
    // need to add Root indirect reference
    // need to add byte offset to cross reference

    return new PdfExporterResults();
  }

  public static PdfDocument Load(string file) {
    // TODO load from file with pdf loader
    return null;
  }
}