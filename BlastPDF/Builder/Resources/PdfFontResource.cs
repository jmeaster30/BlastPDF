using System;

namespace BlastPDF.Builder.Resources;

public enum PdfFontType
{
    Type0,
    Type1,
    MmType1,
    Type3,
    TrueType,
    CidFontType0,
    CidFontType2
}

public abstract class PdfFontResource: PdfObject
{
    public PdfFontType Subtype { get; set; }

    public PdfFontResource(PdfFontType type)
    {
        Subtype = type;
    }
}

public static class PdfFontResourceExtensions {
    public static PdfPage UseFont(this PdfPage page, string resourceName, PdfFontResource resource)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (page.Fonts.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
        page.Fonts.Add(resourceName, resource);
        return page;
    }
    
    public static PdfDocument UseFont(this PdfDocument document, string resourceName, PdfFontResource resource)
    {
        if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException(nameof(resourceName));
        if (document.Fonts.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
        document.Fonts.Add(resourceName, resource);
        return document;
    }
    
    public static PdfPage UseCourier(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("Courier")) throw new ArgumentException("Resource 'Courier' already exists as a resource for this page :(");
        var font = PdfBaseFont.Courier;
        page.Fonts.Add("Courier", font);
        return page;
    }
    
    public static PdfDocument UseCourier(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("Courier")) throw new ArgumentException("Resource 'Courier' already exists as a resource for this page :(");
        var font = PdfBaseFont.Courier;
        document.Fonts.Add("Courier", font);
        return document;
    }
    
    public static PdfPage UseCourierBold(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("CourierBold")) throw new ArgumentException("Resource 'CourierBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierBold;
        page.Fonts.Add("CourierBold", font);
        return page;
    }
    
    public static PdfDocument UseCourierBold(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("CourierBold")) throw new ArgumentException("Resource 'CourierBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierBold;
        document.Fonts.Add("CourierBold", font);
        return document;
    }
    
    public static PdfPage UseCourierBoldOblique(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("CourierBoldOblique")) throw new ArgumentException("Resource 'CourierBoldOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierBoldOblique;
        page.Fonts.Add("CourierBoldOblique", font);
        return page;
    }
    
    public static PdfDocument UseCourierBoldOblique(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("CourierBoldOblique")) throw new ArgumentException("Resource 'CourierBoldOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierBoldOblique;
        document.Fonts.Add("CourierBoldOblique", font);
        return document;
    }
    
    public static PdfPage UseCourierOblique(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("CourierOblique")) throw new ArgumentException("Resource 'CourierOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierOblique;
        page.Fonts.Add("CourierOblique", font);
        return page;
    }
    
    public static PdfDocument UseCourierOblique(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("CourierOblique")) throw new ArgumentException("Resource 'CourierOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.CourierOblique;
        document.Fonts.Add("CourierOblique", font);
        return document;
    }
    
    public static PdfPage UseHelvetica(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("Helvetica")) throw new ArgumentException("Resource 'Helvetica' already exists as a resource for this page :(");
        var font = PdfBaseFont.Helvetica;
        page.Fonts.Add("Helvetica", font);
        return page;
    }
    
    public static PdfDocument UseHelvetica(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("Helvetica")) throw new ArgumentException("Resource 'Helvetica' already exists as a resource for this page :(");
        var font = PdfBaseFont.Helvetica;
        document.Fonts.Add("Helvetica", font);
        return document;
    }
    
    public static PdfPage UseHelveticaBold(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("HelveticaBold")) throw new ArgumentException("Resource 'HelveticaBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaBold;
        page.Fonts.Add("HelveticaBold", font);
        return page;
    }
    
    public static PdfDocument UseHelveticaBold(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("HelveticaBold")) throw new ArgumentException("Resource 'HelveticaBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaBold;
        document.Fonts.Add("HelveticaBold", font);
        return document;
    }
    
    public static PdfPage UseHelveticaBoldOblique(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("HelveticaBoldOblique")) throw new ArgumentException("Resource 'HelveticaBoldOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaBoldOblique;
        page.Fonts.Add("HelveticaBoldOblique", font);
        return page;
    }
    
    public static PdfDocument UseHelveticaBoldOblique(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("HelveticaBoldOblique")) throw new ArgumentException("Resource 'HelveticaBoldOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaBoldOblique;
        document.Fonts.Add("HelveticaBoldOblique", font);
        return document;
    }
    
    public static PdfPage UseHelveticaOblique(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("HelveticaOblique")) throw new ArgumentException("Resource 'HelveticaOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaOblique;
        page.Fonts.Add("HelveticaOblique", font);
        return page;
    }
    
    public static PdfDocument UseHelveticaOblique(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("HelveticaOblique")) throw new ArgumentException("Resource 'HelveticaOblique' already exists as a resource for this page :(");
        var font = PdfBaseFont.HelveticaOblique;
        document.Fonts.Add("HelveticaOblique", font);
        return document;
    }
    
    public static PdfPage UseTimesNewRoman(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("TimesNewRoman")) throw new ArgumentException("Resource 'TimesNewRoman' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRoman;
        page.Fonts.Add("TimesNewRoman", font);
        return page;
    }
    
    public static PdfDocument UseTimesNewRoman(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("TimesNewRoman")) throw new ArgumentException("Resource 'TimesNewRoman' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRoman;
        document.Fonts.Add("TimesNewRoman", font);
        return document;
    }
    
    public static PdfPage UseTimesNewRomanBold(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("TimesNewRomanBold")) throw new ArgumentException("Resource 'TimesNewRomanBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanBold;
        page.Fonts.Add("TimesNewRomanBold", font);
        return page;
    }
    
    public static PdfDocument UseTimesNewRomanBold(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("TimesNewRomanBold")) throw new ArgumentException("Resource 'TimesNewRomanBold' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanBold;
        document.Fonts.Add("TimesNewRomanBold", font);
        return document;
    }
    
    public static PdfPage UseTimesNewRomanItalic(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("TimesNewRomanItalic")) throw new ArgumentException("Resource 'TimesNewRomanItalic' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanItalic;
        page.Fonts.Add("TimesNewRomanItalic", font);
        return page;
    }
    
    public static PdfDocument UseTimesNewRomanItalic(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("TimesNewRomanItalic")) throw new ArgumentException("Resource 'TimesNewRomanItalic' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanItalic;
        document.Fonts.Add("TimesNewRomanItalic", font);
        return document;
    }
    
    public static PdfPage UseTimesNewRomanBoldItalic(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("TimesNewRomanBoldItalic")) throw new ArgumentException("Resource 'TimesNewRomanBoldItalic' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanBoldItalic;
        page.Fonts.Add("TimesNewRomanBoldItalic", font);
        return page;
    }
    
    public static PdfDocument UseTimesNewRomanBoldItalic(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("TimesNewRomanBoldItalic")) throw new ArgumentException("Resource 'TimesNewRomanBoldItalic' already exists as a resource for this page :(");
        var font = PdfBaseFont.TimesNewRomanBoldItalic;
        document.Fonts.Add("TimesNewRomanBoldItalic", font);
        return document;
    }
    
    public static PdfPage UseSymbolFont(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("Symbol")) throw new ArgumentException("Resource 'Symbol' already exists as a resource for this page :(");
        var font = PdfBaseFont.Symbol;
        page.Fonts.Add("Symbol", font);
        return page;
    }
    
    public static PdfDocument UseSymbolFont(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("Symbol")) throw new ArgumentException("Resource 'Symbol' already exists as a resource for this page :(");
        var font = PdfBaseFont.Symbol;
        document.Fonts.Add("Symbol", font);
        return document;
    }
    
    public static PdfPage UseZapfDingbats(this PdfPage page)
    {
        if (page.Fonts.ContainsKey("ZapfDingbats")) throw new ArgumentException("Resource 'ZapfDingbats' already exists as a resource for this page :(");
        var font = PdfBaseFont.ZapfDingbats;
        page.Fonts.Add("ZapfDingbats", font);
        return page;
    }
    
    public static PdfDocument UseZapfDingbats(this PdfDocument document)
    {
        if (document.Fonts.ContainsKey("ZapfDingbats")) throw new ArgumentException("Resource 'ZapfDingbats' already exists as a resource for this page :(");
        var font = PdfBaseFont.ZapfDingbats;
        document.Fonts.Add("ZapfDingbats", font);
        return document;
    }
}