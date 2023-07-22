using System;

namespace BlastPDF.Builder.Resources;

public static class PdfResource
{
    public static PdfPage UseObject(this PdfPage page, string resourceName, PdfObject resource)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (page.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
        page.Resources.Add(resourceName, resource);
        return page;
    }
    
    public static PdfDocument UseObject(this PdfDocument doc, string resourceName, PdfObject resource)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (doc.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this document :(");
        doc.Resources.Add(resourceName, resource);
        return doc;
    }
}