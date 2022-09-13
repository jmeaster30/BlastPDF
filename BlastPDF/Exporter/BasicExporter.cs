using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlastPDF.Exporter.Util;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;

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
    
    // export the page resources

    var nextStart = objectNumber;
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
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n\n".ToUTF8());

    return results;
  }

  private static PdfExporterResults Export(this PdfGraphicsObject graphicsObject, Stream stream, int objectNumber) {
    // TODO really need to make this better
    PdfGraphicsGroup group = graphicsObject as PdfGraphicsGroup;
    if (group != null) return group.Export(stream, objectNumber);
    PdfPathMove move = graphicsObject as PdfPathMove;
    if (move != null) return move.Export(stream, objectNumber);
    PdfPathLine line = graphicsObject as PdfPathLine;
    if (line != null) return line.Export(stream, objectNumber);
    PdfPathBezier bezier = graphicsObject as PdfPathBezier;
    if (bezier != null) return bezier.Export(stream, objectNumber);
    PdfPathRect rect = graphicsObject as PdfPathRect;
    if (rect != null) return rect.Export(stream, objectNumber);
    PdfPathClose close = graphicsObject as PdfPathClose;
    if (close != null) return close.Export(stream, objectNumber);
    PdfPathPaint paint = graphicsObject as PdfPathPaint;
    if (paint != null) return paint.Export(stream, objectNumber);
    PdfColor color = graphicsObject as PdfColor;
    if (color != null) return color.Export(stream, objectNumber);
    PdfTransform transform = graphicsObject as PdfTransform;
    if (transform != null) return transform.Export(stream, objectNumber);
    PdfLineWidth lineWidth = graphicsObject as PdfLineWidth;
    if (lineWidth != null) return lineWidth.Export(stream, objectNumber);
    PdfLineCapStyle capStyle = graphicsObject as PdfLineCapStyle;
    if (capStyle != null) return capStyle.Export(stream, objectNumber);
    PdfLineJoinStyle joinStyle = graphicsObject as PdfLineJoinStyle;
    if (joinStyle != null) return joinStyle.Export(stream, objectNumber);
    PdfMiterLimit miterLimit = graphicsObject as PdfMiterLimit;
    if (miterLimit != null) return miterLimit.Export(stream, objectNumber);
    PdfLineDashPattern dashPattern = graphicsObject as PdfLineDashPattern;
    if (dashPattern != null) return dashPattern.Export(stream, objectNumber);
    PdfRenderingIntent intent = graphicsObject as PdfRenderingIntent;
    if (intent != null) return intent.Export(stream, objectNumber);
    PdfResetGraphicsState reset = graphicsObject as PdfResetGraphicsState;
    if (reset != null) return reset.Export(stream, objectNumber);
    
    foreach (var obj in graphicsObject.SubObjects) {
      obj.Export(stream, objectNumber);
    }
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
      default:
        break;
    }
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfTransform transform, Stream stream, int objectNumber) {
    stream.Write($"{transform.Value[0]} {transform.Value[1]} {transform.Value[2]} {transform.Value[3]} {transform.Value[4]} {transform.Value[5]} cm\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineWidth lineWidth, Stream stream, int objectNumber) {
    stream.Write($"{lineWidth.Width} w\n".ToUTF8());
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineCapStyle lineCapStyle, Stream stream, int objectNumber) {
    stream.Write(lineCapStyle.LineCapStyle switch {
      LineCapStyle.SquareButt => $"0 J\n".ToUTF8(),
      LineCapStyle.Round => $"1 J\n".ToUTF8(),
      LineCapStyle.ProjectingSquare => $"2 J\n".ToUTF8(),
      _ => $"0 J\n".ToUTF8()
    });
    return new PdfExporterResults();
  }

  private static PdfExporterResults Export(this PdfLineJoinStyle joinStyle, Stream stream, int objectNumber) {
    stream.Write(joinStyle.LineJoinStyle switch {
      LineJoinStyle.Mitered => $"0 J\n".ToUTF8(),
      LineJoinStyle.Round => $"1 J\n".ToUTF8(),
      LineJoinStyle.Bevel => $"2 J\n".ToUTF8(),
      _ => $"0 J\n".ToUTF8()
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

}

internal class PdfExporterResults {
  public List<(int, long)> ObjectNumberByteOffsets = new List<(int, long)>();
  public List<int> PageRefNumber = new List<int>();

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