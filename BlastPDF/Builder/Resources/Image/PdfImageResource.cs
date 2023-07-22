using System;
using BlastPDF.Builder.Graphics;
using BlastPDF.Filter;
using SharperImage;
using SharperImage.Formats;

namespace BlastPDF.Builder.Resources.Image;

public class PdfImageResource : PdfObject
{
    public uint Width { get; set; }
    public uint Height { get; set; }
    public PdfColorSpace ColorSpace { get; set; }
    public int BitsPerComponent { get; set; }
    public PdfFilter[] Filters { get; set; }
    
    public IImage ImageData { get; set; }
    
    public static PdfImageResource FromFile(string filename, FileFormat format, PdfColorSpace colorSpace, PdfFilter[] filters)
    {
        var image = IImage.Decode(filename, format);
        return FromImage(image, colorSpace, filters);
    }
  
    public static PdfImageResource FromImage(IImage image, PdfColorSpace colorSpace, PdfFilter[] filters)
    {
        var result = new PdfImageResource {
            Width = image.Width(),
            Height = image.Height(),
            ColorSpace = colorSpace,
            BitsPerComponent = 8, // TODO I would like this to be figured out from the pixel format in the image
            ImageData = image,
            Filters = filters,
        };

        if (filters is null)
        {
            result.Filters = new[] {PdfFilter.AsciiHex};
        }

        return result;
    }
}

public static class PdfImageResourceExtensions
{
    public static PdfPage UseImage(this PdfPage page, string resourceName, string filename, FileFormat format, PdfColorSpace colorSpace = PdfColorSpace.DeviceRGB, PdfFilter[] filters = null)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (page.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
        page.Resources.Add(resourceName, PdfImageResource.FromFile(filename, format, colorSpace, filters));
        return page;
    }
    
    public static PdfDocument UseImage(this PdfDocument doc, string resourceName, string filename, FileFormat format, PdfColorSpace colorSpace = PdfColorSpace.DeviceRGB, PdfFilter[] filters = null)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (doc.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this document :(");
        doc.Resources.Add(resourceName, PdfImageResource.FromFile(filename, format, colorSpace, filters));
        return doc;
    }
    
    public static PdfPage UseImage(this PdfPage page, string resourceName, IImage image, PdfColorSpace colorSpace = PdfColorSpace.DeviceRGB, PdfFilter[] filters = null)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (page.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
        page.Resources.Add(resourceName, PdfImageResource.FromImage(image, colorSpace, filters));
        return page;
    }
    
    public static PdfDocument UseImage(this PdfDocument doc, string resourceName, IImage image, PdfColorSpace colorSpace = PdfColorSpace.DeviceRGB, PdfFilter[] filters = null)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (doc.Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this document :(");
        doc.Resources.Add(resourceName, PdfImageResource.FromImage(image, colorSpace, filters));
        return doc;
    }
}