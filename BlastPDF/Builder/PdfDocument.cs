using System.Collections.Generic;

namespace BlastPDF.Builder;

public class PdfDocument {
  public List<PdfPage> Pages { get; } = new List<PdfPage>();

  public static PdfDocument Create() {
    return new PdfDocument();
  }

  public PdfDocument AddPage(PdfPage page) {
    Pages.Add(page);
    return this;
  }
}