using System;
using System.Collections.Generic;
using BlastPDF.Builder.Resources;
using BlastPDF.Builder.Resources.Font;

namespace BlastPDF.Builder;

public class PdfDocument {
  public List<PdfPage> Pages { get; } = new();
  public Dictionary<string, PdfObject> Resources { get; } = new();
  public Dictionary<string, PdfFontResource> Fonts { get; } = new();

  // create object at end of document and add it to} the trailer as /Info
  public Dictionary<string, IPdfValue> Metadata { get; } = new();

  public static PdfDocument Create() {
    return new PdfDocument();
  }

  public PdfDocument AddResource(string resourceName, PdfObject pdfObject) {
    if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
    if (Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
    Resources.Add(resourceName, pdfObject);
    return this;
  }

  public PdfDocument AddPage(PdfPage page) {
    Pages.Add(page);
    return this;
  }

  public PdfDocument AddMetadata(string name, IPdfValue value)
  {
    Metadata.Add(name, value);
    return this;
  }

  public PdfDocument AddMetadata(Dictionary<string, IPdfValue> metadata)
  {
    foreach (var value in metadata)
    {
      Metadata.Add(value.Key, value.Value);
    }

    return this;
  }
}