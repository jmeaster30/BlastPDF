using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlastIMG;
using BlastPDF.Exporter.Util;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using BlastSharp.Lists;

namespace BlastPDF.Exporter.Basic;

public static class BasicExporterExtension {

  public static void Save(this PdfDocument document, Stream stream) {
    stream.Write("%PDF-1.7\n".ToUTF8());
    stream.Write("%%EOF\n\n".ToUTF8());

    var crossReferences = new List<(int, long)>();
    var pageRefNumbers = new List<int>();

    var nextStart = 3;

    foreach(var page in document.Pages) {
      var refs = page.Export(stream, nextStart);
      crossReferences.AddRange(refs.ObjectNumberByteOffsets);
      if (refs.ObjectNumberByteOffsets.Any()) {
        nextStart = refs.ObjectNumberByteOffsets.Select(x => x.Item1).OrderBy(x => x).FirstOrDefault() + 1;
      }
      pageRefNumbers.AddRange(refs.PageRefNumber);
    }

    // document level resources

    crossReferences.Add((2, stream.Position));
    stream.Write($"2 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Pages\n".ToUTF8());
    stream.Write("/Kids [\n".ToUTF8());
    foreach(var pageRef in pageRefNumbers) {
      stream.Write($"{pageRef} 0 R\n".ToUTF8());
    }
    stream.Write("]\n".ToUTF8());
    stream.Write($"/Count {pageRefNumbers.Count}\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n".ToUTF8());

    crossReferences.Add((1, stream.Position));
    stream.Write($"1 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Catalog\n".ToUTF8());
    stream.Write($"/Pages 2 0 R\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n".ToUTF8());

    // cross reference

    var xrefByteOffset = stream.Position;
    stream.Write("xref\n".ToUTF8());
    stream.Write($"0 {crossReferences.Count + 1}\n".ToUTF8());

    crossReferences.Sort((a, b) => a.Item1 - b.Item1);
    stream.Write($"0000000000 65535 f\n".ToUTF8());
    foreach(var xref in crossReferences) {
      stream.Write($"{xref.Item2.ToString().PadLeft(10, '0')} 00000 n\n".ToUTF8());
    }
    stream.Write("trailer\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write($"/Size {crossReferences.Count + 1}\n".ToUTF8());
    stream.Write($"/Root 1 0 R\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("startxref\n".ToUTF8());
    stream.Write($"{xrefByteOffset}\n".ToUTF8());
    stream.Write("%%EOF\n".ToUTF8());
  }

  private static PdfExporterResults Export(this PdfPage page, Stream stream, int objectNumber) {
    var results = new PdfExporterResults();

    var crossReferences = new List<(int, long)>();
    var contentRefs = new List<int>();
    var nextStart = objectNumber;
    
    // TODO Must allow more than one font
    crossReferences.Add((nextStart, stream.Position));
    stream.Write($"{nextStart} 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Font\n".ToUTF8());
    stream.Write("/SubType /Type\n".ToUTF8());
    stream.Write("/Name /Default\n".ToUTF8());
    stream.Write("/BaseFont /Helvetica\n".ToUTF8());
    stream.Write("/Encoding /WinAnsiEncoding\n".ToUTF8());
    stream.Write(">>\nendobj\n".ToUTF8());
    var fontResourceStart = nextStart;
    nextStart += 1;
    
    // export the page resources
    List<(string, int)> x_objects = new();
    foreach (var res in page.Resources)
    {
      x_objects.Add((res.Key, nextStart));
      res.Value.Export(stream, nextStart);
      nextStart += 1;
    }
    // build page resource dictionary
    var pageResources = nextStart;
    crossReferences.Add((nextStart, stream.Position));
    stream.Write($"{nextStart} 0 obj\n".ToUTF8());
    stream.Write("<< /ProcSet [/PDF /Text /ImageB /ImageC /ImageI]\n".ToUTF8());
    stream.Write("/XObject <<\n".ToUTF8());
    foreach (var xobj in x_objects)
    {
      stream.Write($"/{xobj.Item1} {xobj.Item2} 0 R\n".ToUTF8());
    }
    // Font's will be in a different sub dictionary in this resource section
    stream.Write(">>\n".ToUTF8());
    stream.Write("/Font <<\n".ToUTF8());
    stream.Write($"/F1 {fontResourceStart} 0 R\n".ToUTF8()); // TODO Must allow more than one font
    stream.Write(">>\n".ToUTF8());
    stream.Write(">>\nendobj\n".ToUTF8());

    nextStart += 1;
    
    foreach(var obj in page.Objects) {
      contentRefs.Add(nextStart);
      crossReferences.Add((nextStart, stream.Position));
      stream.Write($"{nextStart} 0 obj\n".ToUTF8());
      stream.Write("<<\n".ToUTF8());
      stream.Write($"/Length {nextStart + 1} 0 R\n".ToUTF8());
      stream.Write(">>\n".ToUTF8());
      stream.Write("stream\n".ToUTF8());

      var startOffset = stream.Position;
      stream.Write("q\n".ToUTF8());
      var refs = obj.Export(stream, nextStart + 2);
      // We shouldn't need to do anything with refs currently
      stream.Write("Q\n".ToUTF8());
      var endOffset = stream.Position;
      stream.Write("endstream\nendobj\n".ToUTF8());

      crossReferences.Add((nextStart + 1, stream.Position));
      stream.Write($"{nextStart + 1} 0 obj\n".ToUTF8());
      stream.Write($"({endOffset - startOffset})\n".ToUTF8());
      stream.Write("endobj\n\n".ToUTF8());
      nextStart += 2;
      //crossReferences.AddRange(refs.ObjectNumberByteOffsets);
      //if (refs.ObjectNumberByteOffsets.Any()) {
      //  nextStart = refs.ObjectNumberByteOffsets.Select(x => x.Item1).OrderBy(x => x).FirstOrDefault() + 1;
      //}
    }
    
    

    results.AddObjects(crossReferences);

    results.AddObject(nextStart, stream.Position);
    results.AddPageObject(nextStart);
    stream.Write($"{nextStart} 0 obj\n".ToUTF8());
    stream.Write("<<\n".ToUTF8());
    stream.Write("/Type /Page\n".ToUTF8());
    stream.Write("/Parent 2 0 R\n".ToUTF8());
    stream.Write($"/MediaBox [ {page.CropBoxX} {page.CropBoxY} {page.CropBoxW} {page.CropBoxH} ]\n".ToUTF8());
    stream.Write("/Contents [\n".ToUTF8());
    foreach(var contentRef in contentRefs) {
      stream.Write($"{contentRef} 0 R\n".ToUTF8());
    }
    stream.Write("]\n".ToUTF8());
    stream.Write($"/Resources {pageResources} 0 R\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n\n".ToUTF8());

    return results;
  }

  private static PdfExporterResults Export(this PdfObject pdfObject, Stream stream, int objectNumber)
  {
    return pdfObject switch
    {
      PdfGraphicsObject graphics => graphics.Export(stream, objectNumber),
      _ => throw new Exception("Unhandled subtype of PdfObject")
    };
  }

  /*private static PdfExporterResults Export(this PdfEmbeddedImage embed, Stream stream, int objectNumber)
  {
    stream.Write($"{objectNumber} 0 obj\n".ToUTF8());
    stream.Write("<<\n/Type /XObject\n/Subtype /Image\n".ToUTF8());
    stream.Write($"/Width {embed.Width}\n/Height {embed.Height}".ToUTF8());
    stream.Write($"/ColorSpace /{embed.ColorSpace}\n/BitsPerComponent {embed.BitsPerComponent}\n".ToUTF8());
    stream.Write($"/Length {embed.ImageData.LongLength}\n".ToUTF8());
    stream.Write(">>\nstream\n".ToUTF8());
    stream.Write(embed.ImageData);
    stream.Write("\nendstream\nendobj\n".ToUTF8());
    return new PdfExporterResults();
  }*/

  private static PdfExporterResults Export(this PdfGraphicsObject graphicsObject, Stream stream, int objectNumber) {
    // TODO I have a feeling this could still be better
    switch (graphicsObject)
    {
      case PdfGraphicsGroup group: return group.Export(stream, objectNumber);
      case PdfPathMove move: return move.Export(stream, objectNumber);
      case PdfPathLine line: return line.Export(stream, objectNumber);
      case PdfPathBezier bezier: return bezier.Export(stream, objectNumber);
      case PdfPathRect rect: return rect.Export(stream, objectNumber);
      case PdfPathClose close: return close.Export(stream, objectNumber);
      case PdfPathPaint paint: return paint.Export(stream, objectNumber);
      case PdfColor color: return color.Export(stream, objectNumber);
      case PdfTransform transform: return transform.Export(stream, objectNumber);
      case PdfLineWidth lineWidth: return lineWidth.Export(stream, objectNumber);
      case PdfLineCapStyle capStyle: return capStyle.Export(stream, objectNumber);
      case PdfLineJoinStyle joinStyle: return joinStyle.Export(stream, objectNumber);
      case PdfMiterLimit miterLimit: return miterLimit.Export(stream, objectNumber);
      case PdfLineDashPattern dashPattern: return dashPattern.Export(stream, objectNumber);
      case PdfRenderingIntent intent: return intent.Export(stream, objectNumber);
      case PdfResetGraphicsState reset: return reset.Export(stream, objectNumber);
      case PdfXObject xobj: return xobj.Export(stream, objectNumber);
      case PdfInlineImage image: return image.Export(stream, objectNumber);
      case PdfTextObject text: return text.Export(stream, objectNumber);
    }

    foreach (var obj in graphicsObject.SubObjects) {
      obj.Export(stream, objectNumber);
    }
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfTextObject textObject, Stream stream, int objectNumber) {
    // TODO I have a feeling this could still be better
    switch (textObject)
    {
      case PdfShowText showText: return showText.Export(stream, objectNumber);
      case PdfShowTextNextLine showTextNextLine: return showTextNextLine.Export(stream, objectNumber);
      case PdfShowTextSpacingNextLine showTextSpacingNextLine: return showTextSpacingNextLine.Export(stream, objectNumber);
      case PdfShowTextList showTextList: return showTextList.Export(stream, objectNumber);
      case PdfNextLine nextLine: return nextLine.Export(stream, objectNumber);
      case PdfNextLineOffset nextLineOffset: return nextLineOffset.Export(stream, objectNumber);
      case PdfNextLineOffsetLeading nextLineOffsetLeading: return nextLineOffsetLeading.Export(stream, objectNumber);
      case PdfTextTransform nextLineTextTransform: return nextLineTextTransform.Export(stream, objectNumber);
      case PdfCharacterSpacing characterSpacing: return characterSpacing.Export(stream, objectNumber);
      case PdfWordSpacing wordSpacing: return wordSpacing.Export(stream, objectNumber);
      case PdfTextHorizontalScale horizontalScale: return horizontalScale.Export(stream, objectNumber);
      case PdfTextLeading textLeading: return textLeading.Export(stream, objectNumber);
      case PdfSetFont setFont: return setFont.Export(stream, objectNumber);
      case PdfSetTextRenderingMode renderingMode: return renderingMode.Export(stream, objectNumber);
      case PdfTextRise textRise: return textRise.Export(stream, objectNumber);
    }
    
    stream.Write("BT\n".ToUTF8());
    foreach (var obj in textObject.SubObjects) {
      obj.Export(stream, objectNumber);
    }
    stream.Write("ET\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfInlineImage image, Stream stream, int objectNumber)
  {
    Console.WriteLine("HERE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    stream.Write("BI\n".ToUTF8());
    stream.Write($"\t/W {image.Width}\n".ToUTF8());
    stream.Write($"\t/H {image.Height}\n".ToUTF8());
    // TODO this should be extracted to an extension function
    var colorSpaceString = image.ColorSpace switch
    {
      PdfColorSpace.DeviceGray => "G",
      PdfColorSpace.DeviceRGB => "RGB",
      PdfColorSpace.DeviceCMYK => "CMYK",
      _ => throw new NotImplementedException(),
    };
    stream.Write($"\t/CS /{colorSpaceString}\n".ToUTF8());
    stream.Write($"\t/BPC {image.BitsPerComponent}\n".ToUTF8());
    var filterNames = image.Filters.Select(x => x switch
    {
      PdfFilter.ASCII85 => "/A85",
      PdfFilter.ASCIIHex => "/AHx",
      PdfFilter.LZW => "/LZW",
      PdfFilter.RunLength => "/RL",
      _ => throw new NotImplementedException()
    }).Join(" ");
    stream.Write($"\t/F [{filterNames}]\n".ToUTF8());
    stream.Write("ID\n".ToUTF8());

    IEnumerable<byte> imageData = image.ImageData.GetColorArray(image.ColorSpace switch
    {
      PdfColorSpace.DeviceGray => ColorFormat.GRAY,
      PdfColorSpace.DeviceRGB => ColorFormat.RGB,
      PdfColorSpace.DeviceCMYK => ColorFormat.CMYK,
      _ => throw new NotImplementedException(),
    });

    imageData = image.Filters.Reverse().Aggregate(imageData, (current, filter) => filter.Encode(current));

    stream.Write(imageData.ToArray());
    
    stream.Write("\nEI\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfXObject xobj, Stream stream, int objectNumber)
  {
    stream.Write($"/{xobj.Resource} Do\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfGraphicsGroup group, Stream stream, int objectNumber) {
    stream.Write("q\n".ToUTF8());
    foreach (var obj in group.SubObjects) {
      obj.Export(stream, objectNumber);
    }
    stream.Write("Q\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathMove move, Stream stream, int objectNumber) {
    stream.Write($"{move.X} {move.Y} m\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathLine line, Stream stream, int objectNumber) {
    stream.Write($"{line.X} {line.Y} l\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathBezier bezier, Stream stream, int objectNumber) {
    stream.Write($"{bezier.Control1X} {bezier.Control1Y} {bezier.Control2X} {bezier.Control2Y} {bezier.DestX} {bezier.DestY} c\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathRect rect, Stream stream, int objectNumber) {
    stream.Write($"{rect.X} {rect.Y} {rect.Width} {rect.Height} re\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathClose close, Stream stream, int objectNumber) {
    stream.Write("h\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfPathPaint paint, Stream stream, int objectNumber) {
    stream.Write($"{paint.PaintMode.getOperator()}\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfColor color, Stream stream, int objectNumber) {
    bool isStroke = color.StrokeColor.Any();
    switch (color.ColorSpace) {
      case PdfColorSpace.DeviceGray:
        if (isStroke) {
          stream.Write($"{color.StrokeColor[0]} G\n".ToUTF8());
        } else {
          stream.Write($"{color.NonStrokeColor[0]} g\n".ToUTF8());
        }
        break;
      case PdfColorSpace.DeviceRGB:
        if (isStroke) {
          stream.Write($"{color.StrokeColor[0]} {color.StrokeColor[1]} {color.StrokeColor[2]} RG\n".ToUTF8());
        } else {
          stream.Write($"{color.NonStrokeColor[0]} {color.NonStrokeColor[1]} {color.NonStrokeColor[2]} rg\n".ToUTF8());
        }
        break;
      case PdfColorSpace.DeviceCMYK:
        if (isStroke) {
          stream.Write($"{color.StrokeColor[0]} {color.StrokeColor[1]} {color.StrokeColor[2]} {color.StrokeColor[3]} K\n".ToUTF8());
        } else {
          stream.Write($"{color.NonStrokeColor[0]} {color.NonStrokeColor[1]} {color.NonStrokeColor[2]} {color.NonStrokeColor[3]} k\n".ToUTF8());
        }
        break;
    }
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfTransform transform, Stream stream, int objectNumber) {
    stream.Write($"{transform.Value[0]} {transform.Value[1]} {transform.Value[2]} {transform.Value[3]} {transform.Value[4]} {transform.Value[5]} cm\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfTextTransform transform, Stream stream, int objectNumber) {
    stream.Write($"{transform.Value[0]} {transform.Value[1]} {transform.Value[2]} {transform.Value[3]} {transform.Value[4]} {transform.Value[5]} cm\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineWidth lineWidth, Stream stream, int objectNumber) {
    stream.Write($"{lineWidth.Width} w\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineCapStyle lineCapStyle, Stream stream, int objectNumber) {
    stream.Write(lineCapStyle.LineCapStyle switch {
      LineCapStyle.SquareButt => "0 J\n".ToUTF8(),
      LineCapStyle.Round => "1 J\n".ToUTF8(),
      LineCapStyle.ProjectingSquare => "2 J\n".ToUTF8(),
      _ => "0 J\n".ToUTF8()
    });
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineJoinStyle joinStyle, Stream stream, int objectNumber) {
    stream.Write(joinStyle.LineJoinStyle switch {
      LineJoinStyle.Mitered => "0 J\n".ToUTF8(),
      LineJoinStyle.Round => "1 J\n".ToUTF8(),
      LineJoinStyle.Bevel => "2 J\n".ToUTF8(),
      _ => "0 J\n".ToUTF8()
    });
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfMiterLimit miterLimit, Stream stream, int objectNumber) {
    stream.Write($"{miterLimit.Limit} M\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineDashPattern dashPattern, Stream stream, int objectNumber) {
    stream.Write("[ ".ToUTF8());
    foreach (var dash in dashPattern.Array) {
      stream.Write($"{dash} ".ToUTF8());
    }
    stream.Write($"] {dashPattern.Phase} d\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfRenderingIntent renderingIntent, Stream stream, int objectNumber) {
    stream.Write($"/{renderingIntent.RenderingIntent} ri\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfResetGraphicsState reset, Stream stream, int objectNumber) {
    stream.Write("Q\n".ToUTF8());
    stream.Write("q\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfShowText showText, Stream stream, int objectNumber) {
    stream.Write($"( {showText.Value} ) Tj\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfShowTextNextLine showText, Stream stream, int objectNumber) {
    stream.Write($"( {showText.Value} ) '\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfShowTextSpacingNextLine showText, Stream stream, int objectNumber) {
    stream.Write($"{showText.Width} ( {showText.Value} ) \"\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfShowTextList showText, Stream stream, int objectNumber)
  {
    throw new NotImplementedException("Export show text list");
  }
  
  private static PdfExporterResults Export(this PdfNextLine nextLine, Stream stream, int objectNumber) {
    stream.Write($"T*\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfNextLineOffset nextLine, Stream stream, int objectNumber) {
    stream.Write($"{nextLine.OffsetX} {nextLine.OffsetY} Td\n".ToUTF8());
    return new PdfExporterResults();
  }
  
  private static PdfExporterResults Export(this PdfNextLineOffsetLeading nextLine, Stream stream, int objectNumber) {
    stream.Write($"{nextLine.OffsetX} {nextLine.OffsetY} TD\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfCharacterSpacing characterSpacing, Stream stream, int objectNumber)
  {
    stream.Write($"{characterSpacing.CharSpace} Tc\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfWordSpacing wordSpacing, Stream stream, int objectNumber)
  {
    stream.Write($"{wordSpacing.WordSpace} Tw\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfTextHorizontalScale horizontalScale, Stream stream, int objectNumber)
  {
    stream.Write($"{horizontalScale.Scale} Tz\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfTextLeading leading, Stream stream, int objectNumber)
  {
    stream.Write($"{leading.Leading} TL\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfSetFont font, Stream stream, int objectNumber)
  {
    stream.Write($"/{font.FontName} {font.FontSize} Tf\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfSetTextRenderingMode renderingMode, Stream stream, int objectNumber)
  {
    stream.Write($"{(int)renderingMode.Mode} Tr\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfTextRise textRise, Stream stream, int objectNumber)
  {
    stream.Write($"{textRise.Amount} Ts\n".ToUTF8());
    return new PdfExporterResults();
  }
}

internal class PdfExporterResults {
  public List<(int, long)> ObjectNumberByteOffsets = new();
  public List<int> PageRefNumber = new();

  public void AddObject(int objectNumber, long byteOffset) {
    ObjectNumberByteOffsets.Add((objectNumber, byteOffset));
  }

  public void AddObjects(List<(int, long)> objs) {
    ObjectNumberByteOffsets.AddRange(objs);
  }

  public void AddPageObject(int objectNumber) {
    PageRefNumber.Add(objectNumber);
  }
}