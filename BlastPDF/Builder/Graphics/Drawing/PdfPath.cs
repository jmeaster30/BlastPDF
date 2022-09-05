using BlastPDF.Builder.Exporter;
using System.IO;
using System.Collections.Generic;
using BlastPDF.Builder.Util;

namespace BlastPDF.Builder.Graphics.Drawing;

abstract class PdfPathSegment {
  public abstract PdfExporterResults Export(Stream stream, int objectNumber);
}

class PdfPathMove : PdfPathSegment {
  decimal _x;
  decimal _y;
  public PdfPathMove(decimal x, decimal y) {
    _x = x; _y = y;
  }

  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    stream.Write($"{_x} {_y} m\n".ToUTF8());
    return new PdfExporterResults();
  }
}

class PdfPathLine : PdfPathSegment {
  decimal _x;
  decimal _y;
  public PdfPathLine(decimal x, decimal y) {
    _x = x; _y = y;
  }

  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    stream.Write($"{_x} {_y} l\n".ToUTF8());
    return new PdfExporterResults();
  }
}

class PdfPathBezier : PdfPathSegment {
  decimal _control1_x;
  decimal _control1_y;
  decimal _control2_x;
  decimal _control2_y;
  decimal _dest_x;
  decimal _dest_y;
  public PdfPathBezier(decimal control1_x, decimal control1_y, decimal control2_x, decimal control2_y, decimal dest_x, decimal dest_y) {
    _control1_x = control1_x; _control1_y = control1_y; 
    _control2_x = control2_x; _control2_y = control2_y; 
    _dest_x = dest_x; _dest_y = dest_y;
  }

  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    stream.Write($"{_control1_x} {_control1_y} {_control2_x} {_control2_y} {_dest_x} {_dest_y} c\n".ToUTF8());
    return new PdfExporterResults();
  }
}

class PdfPathRect : PdfPathSegment {
  decimal _x;
  decimal _y;
  decimal _width;
  decimal _height;
  public PdfPathRect(decimal x, decimal y, decimal width, decimal height) {
    _x = x; _y = y; _width = width; _height = height;
  }

  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    stream.Write($"{_x} {_y} {_width} {_height} re\n".ToUTF8());
    return new PdfExporterResults();
  }
}

class PdfPathClose : PdfPathSegment {
  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    stream.Write("h\n".ToUTF8());
    return new PdfExporterResults();
  }
}

public enum PaintMode {
  Stroke,
  CloseStroke,
  Fill,
  FillEvenOdd,
  FillStroke,
  FillStrokeEvenOdd,
  CloseFillStroke,
  CloseFillStrokeEvenOdd,
  NoStroke
}

public static class PdfPathExtensions {
  public static string getOperator(this PaintMode mode) {
    return mode switch {
      PaintMode.Stroke => "S",
      PaintMode.CloseStroke => "s",
      PaintMode.Fill => "f",
      PaintMode.FillEvenOdd => "f*",
      PaintMode.FillStroke => "B",
      PaintMode.FillStrokeEvenOdd => "B*",
      PaintMode.CloseFillStroke => "b",
      PaintMode.CloseFillStrokeEvenOdd => "b*",
      PaintMode.NoStroke => "n",
      _ => "n"
    };
  }
}

public class PdfPath : PdfGraphicsObject {
  List<PdfPathSegment> segments = new List<PdfPathSegment>();

  PaintMode paintOperator = PaintMode.Stroke;

  public static PdfPath Start() { return new PdfPath(); }
  public PdfPath Move(decimal x, decimal y) {
    segments.Add(new PdfPathMove(x, y));
    return this;
  }

  public PdfPath Line(decimal x, decimal y) {
    segments.Add(new PdfPathLine(x, y));
    return this;
  }

  public PdfPath Bezier(decimal c1x, decimal c1y, decimal c2x, decimal c2y, decimal dx, decimal dy) {
    segments.Add(new PdfPathBezier(c1x, c1y, c2x, c2y, dx, dy));
    return this;
  }

  public PdfPath Close() {
    segments.Add(new PdfPathClose());
    return this;
  }

  public PdfPath Rect(decimal x, decimal y, decimal width, decimal height) {
    segments.Add(new PdfPathRect(x, y, width, height));
    return this;
  }

  public PdfPath Paint(PaintMode paintOp) {
    paintOperator = paintOp;
    return this;
  }

  public override PdfExporterResults Export(Stream stream, int objectNumber) {
    base.Export(stream, objectNumber);
    foreach(var segment in segments) {
      segment.Export(stream, objectNumber);
    }
    stream.Write(paintOperator.getOperator().ToUTF8());
    stream.Write("\n".ToUTF8());

    return new PdfExporterResults();
  }
}