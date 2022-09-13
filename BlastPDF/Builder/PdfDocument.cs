using System;
using System.Collections.Generic;

namespace BlastPDF.Builder;

public class PdfDocument {
  public List<PdfPage> Pages { get; } = new List<PdfPage>();
  public Dictionary<string, PdfObject> Resources { get; } = new Dictionary<string, PdfObject>();

  public static PdfDocument Create() {
    return new PdfDocument();
  }

  public PdfDocument AddResource(string resourceName, PdfObject pdfObject) {
    if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");
    if (Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
    Resources.Add(resourceName, pdfObject);
    return this;
  }

  public PdfDocument AddPage(PdfPage page) {
    Pages.Add(page);
    return this;
  }
}