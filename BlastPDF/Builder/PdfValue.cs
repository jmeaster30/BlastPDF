using System;
using BlastSharp.Text;

namespace BlastPDF.Builder;

public interface IPdfValue
{
    public new string ToString();
}

public class PdfStringValue : IPdfValue
{
    public string Value { get; set; }

    public PdfStringValue(string value)
    {
        Value = value;
    }

    public new string ToString()
    {
        return $"({Value.EscapedString()})";
    }
}

public class PdfDateValue : IPdfValue
{
    public DateTime Value { get; set; }

    public PdfDateValue(DateTime value)
    {
        Value = value;
    }

    public new string ToString()
    {
        // TODO Make a datetime wrapper so the datetime itself manages the timezone
        var offset = TimeZoneInfo.Local.GetUtcOffset(Value);
        return $"(D:{Value.ToString("yyyyMMddHHmmss")}{(offset.Hours < 0 ? '-' : (offset.Hours == 0 ? 'Z' : '+'))}{Math.Abs(offset.Hours):00}'{Math.Abs(offset.Minutes):00})";
    }
}