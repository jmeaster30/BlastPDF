using System.Collections.Generic;

namespace BlastPDF.Builder.Resources;

public class PdfFontType1 : PdfFontResource
{
    public string Name { get; set; }
    public string BaseFont { get; set; }
    //public int? FirstChar { get; set; }
    //public int? LastChar { get; set; }
    //public IEnumerable<int> Widths { get; set; } // required except for the base 14 fonts
    // TODO Dictionary FontDescriptor required except for the base 14 fonts
    public string Encoding { get; set; } = "WinAnsiEncoding";
    // TODO Stream ToUnicode optional

    public PdfFontType1() : base(PdfFontType.Type1) { }
}

public class PdfBaseFont
{
    public static PdfFontResource Courier => new PdfFontType1 { BaseFont = "Courier" };
    public static PdfFontResource CourierBold => new PdfFontType1 { BaseFont = "Courier-Bold" };
    public static PdfFontResource CourierBoldOblique => new PdfFontType1 { BaseFont = "Courier-BoldOblique" };
    public static PdfFontResource CourierOblique => new PdfFontType1 { BaseFont = "Courier" };
    public static PdfFontResource Helvetica => new PdfFontType1 { BaseFont = "Helvetica" };
    public static PdfFontResource HelveticaBold => new PdfFontType1 { BaseFont = "Helvetica-Bold" };
    public static PdfFontResource HelveticaBoldOblique => new PdfFontType1 { BaseFont = "Helvetica-BoldOblique" };
    public static PdfFontResource HelveticaOblique => new PdfFontType1 { BaseFont = "Helvetica-Oblique" };
    public static PdfFontResource TimesNewRoman => new PdfFontType1 { BaseFont = "Times-Roman" };
    public static PdfFontResource TimesNewRomanBold => new PdfFontType1 { BaseFont = "Times-Bold" };
    public static PdfFontResource TimesNewRomanItalic => new PdfFontType1 { BaseFont = "Times-Italic" };
    public static PdfFontResource TimesNewRomanBoldItalic => new PdfFontType1 { BaseFont = "Times-BoldItalic" };
    public static PdfFontResource Symbol => new PdfFontType1 { BaseFont = "Symbol" };
    public static PdfFontResource ZapfDingbats => new PdfFontType1 { BaseFont = "ZapfDingbats" };
}