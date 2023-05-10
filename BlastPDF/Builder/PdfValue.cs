using System;

namespace BlastPDF.Builder;

public interface IPdfValue
{
}

public class PdfStringValue : IPdfValue
{
    public string Value { get; set; }

    public PdfStringValue(string value)
    {
        Value = value;
    }
}

public class PdfDateValue : IPdfValue
{
    public DateTime Value { get; set; }

    public PdfDateValue(DateTime value)
    {
        Value = value;
    }
}