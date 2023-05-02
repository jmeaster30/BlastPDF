using System;
using System.Collections.Generic;

namespace BlastPDF.Builder.Graphics;

public class PdfTextObject : PdfGraphicsObject
{
    public new List<PdfTextObject> SubObjects { get; set; } = new();

    public static PdfTextObject Create()
    {
        return new PdfTextObject();
    }
}

public class PdfCharacterSpacing : PdfTextObject
{
    public decimal CharSpace { get; set; }
}

public class PdfWordSpacing : PdfTextObject
{
    public decimal WordSpace { get; set; }
}

public class PdfTextHorizontalScale : PdfTextObject
{
    public decimal Scale { get; set; }
}

public class PdfTextLeading : PdfTextObject
{
    public decimal Leading { get; set; }
}

public class PdfSetFont : PdfTextObject
{
    public decimal FontSize { get; set; }
    public string FontName { get; set; }
}

public enum PdfTextRenderingMode
{
    Fill,
    Stroke,
    FillStroke,
    Invisible,
    FillClip,
    StrokeClip,
    FillStrokeClip,
    Clip,
}

public class PdfSetTextRenderingMode : PdfTextObject
{
    public PdfTextRenderingMode Mode { get; set; }
}

public class PdfTextRise : PdfTextObject
{
    public decimal Amount { get; set; }
}

public class PdfShowText : PdfTextObject
{
    public string Value { get; set; }
}

public class PdfShowTextNextLine : PdfTextObject
{
    public string Value { get; set; }
}

public class PdfShowTextSpacingNextLine : PdfTextObject
{
    public int Width { get; set; }
    public string Value { get; set; }
}

public class PdfShowTextList : PdfTextObject
{
    // TODO allow widths in here
    public List<string> Values { get; set; }
}

public class PdfNextLine : PdfTextObject { }

public class PdfNextLineOffset : PdfTextObject
{
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}

public class PdfNextLineOffsetLeading : PdfTextObject
{
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
}

public class PdfTextTransform : PdfTextObject
{
    public decimal[] Value { get; private set; }
    
    public static PdfTextTransform Translate(decimal x, decimal y) {
        var result = new PdfTextTransform
        {
            Value = new [] {1.0M, 0.0M, 0.0M, 1.0M, x, y}
        };
        return result;
    }

    public static PdfTextTransform Scale(decimal x, decimal y) {
        var result = new PdfTextTransform
        {
            Value = new [] {x, 0.0M, 0.0M, y, 0.0M, 0.0M}
        };
        return result;
    }

    public static PdfTextTransform Rotate(decimal angle) {
        var result = new PdfTextTransform
        {
            Value = new [] {(decimal)Math.Cos((double)angle), (decimal)Math.Sin((double)angle), -(decimal)Math.Sin((double)angle), (decimal)Math.Cos((double)angle), 0.0M, 0.0M}
        };
        return result;
    }

    public static PdfTextTransform Skew(decimal angle_a, decimal angle_b) {
        var result = new PdfTextTransform
        {
            Value = new [] {1.0M, (decimal)Math.Tan((double)angle_a), (decimal)Math.Tan((double)angle_b), 1.0M, 0.0M, 0.0M}
        };
        return result;
    }
}

public static class PdfTextObjectExtensions
{
    public static PdfTextObject GetTextObject(this PdfGraphicsObject obj)
    {
        if (obj is not PdfTextObject textObject)
        {
            throw new ArgumentException("Not a text object :(");
        }
        return textObject;
    }
    public static PdfTextObject ShowText(this PdfTextObject text, string value)
    {
        text.SubObjects.Add(new PdfShowText
        {
            Value = value,
        });
        return text;
    }
    
    public static PdfTextObject ShowTextNextLine(this PdfTextObject text, string value)
    {
        text.SubObjects.Add(new PdfShowTextNextLine
        {
            Value = value,
        });
        return text;
    }
    
    public static PdfTextObject ShowTextSpacingNextLine(this PdfTextObject text, string value, int width)
    {
        text.SubObjects.Add(new PdfShowTextSpacingNextLine
        {
            Value = value,
            Width = width,
        });
        return text;
    }
    
    public static PdfTextObject NextLine(this PdfTextObject text)
    {
        text.SubObjects.Add(new PdfNextLine());
        return text;
    }
    
    public static PdfTextObject NextLineOffset(this PdfTextObject text, int xOffset, int yOffset)
    {
        text.SubObjects.Add(new PdfNextLineOffset
        {
            OffsetX = xOffset,
            OffsetY = yOffset,
        });
        return text;
    }
    
    public static PdfTextObject NextLineOffsetLeading(this PdfTextObject text, int xOffset, int yOffset)
    {
        text.SubObjects.Add(new PdfNextLineOffsetLeading
        {
            OffsetX = xOffset,
            OffsetY = yOffset,
        });
        return text;
    }
    
    public static PdfTextObject Translate(this PdfTextObject text, decimal x, decimal y) {
        text.SubObjects.Add(PdfTextTransform.Translate(x, y));
        return text;
    }

    public static PdfTextObject Scale(this PdfTextObject text, decimal x, decimal y) {
        text.SubObjects.Add(PdfTextTransform.Scale(x, y));
        return text;
    }

    public static PdfTextObject Rotate(this PdfTextObject text, decimal angle) {
        text.SubObjects.Add(PdfTextTransform.Rotate(angle));
        return text;
    }

    public static PdfTextObject Skew(this PdfTextObject text, decimal angle_a, decimal angle_b) {
        text.SubObjects.Add(PdfTextTransform.Skew(angle_a, angle_b));
        return text;
    }

    public static PdfTextObject CharacterSpacing(this PdfTextObject text, decimal spacing)
    {
        text.SubObjects.Add(new PdfCharacterSpacing
        {
            CharSpace = spacing,
        });
        return text;
    }

    public static PdfTextObject WordSpacing(this PdfTextObject text, decimal spacing)
    {
        text.SubObjects.Add(new PdfWordSpacing
        {
            WordSpace = spacing,
        });
        return text;
    }
    
    public static PdfTextObject TextHorizontalScale(this PdfTextObject text, decimal scale)
    {
        text.SubObjects.Add(new PdfTextHorizontalScale
        {
            Scale = scale,
        });
        return text;
    }
    
    public static PdfTextObject TextLeading(this PdfTextObject text, decimal leading)
    {
        text.SubObjects.Add(new PdfTextLeading
        {
            Leading = leading,
        });
        return text;
    }

    public static PdfTextObject SetFont(this PdfTextObject text, string fontName, decimal size)
    {
        text.SubObjects.Add(new PdfSetFont
        {
            FontName = fontName,
            FontSize = size,
        });
        return text;
    }

    public static PdfTextObject TextRenderingMode(this PdfTextObject text, PdfTextRenderingMode mode)
    {
        text.SubObjects.Add(new PdfSetTextRenderingMode
        {
            Mode = mode,
        });
        return text;
    }

    public static PdfTextObject TextRise(this PdfTextObject text, decimal amount)
    {
        text.SubObjects.Add(new PdfTextRise
        {
            Amount = amount
        });
        return text;
    }
}