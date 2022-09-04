using System.IO;
using System.Collections.Generic;
using BlastPDF.Builder.Interfaces;
using BlastPDF.Builder.Util;

namespace BlastPDF.Builder;

public class PdfDocument : IPdfStreamExporter {
  public List<PdfPage> pages { get; set; }

  public List<int> CrossReferenceTable { get; set; }

  public static PdfDocument Create() {
    return new PdfDocument();
  }

  public PdfDocument AddPage(PdfPage page) {
    page.SetDocument(this);
    pages.Add(page);
    return this;
  }

  public void Export(Stream stream) {
    stream.Write("%PDF-1.7\n".ToUTF8());

    // body
    // root

    // cross reference

    // Trailer
    // need to add sizeof cross reference section
    // need to add Root indirect reference
    // need to add byte offset to cross reference
  }

  public static PdfDocument Load(string file) {
    // TODO load from file with pdf loader
    return null;
  }
}