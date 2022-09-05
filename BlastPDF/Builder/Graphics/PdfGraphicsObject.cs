using BlastPDF.Builder.Graphics.Util;
using BlastPDF.Builder.Exporter;
using BlastPDF.Builder.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlastPDF.Builder.Graphics;

public enum LineCapStyle {
  SquareButt,
  Round,
  ProjectingSquare
}

public enum LineJoinStyle {
  Mitered,
  Round,
  Bevel
}

public class LineDashPattern {
  int[] _array = {};
  int _phase = 0;

  public static LineDashPattern Solid => new LineDashPattern();

  public LineDashPattern() {}

  public LineDashPattern(int rhythm, int phase) {
    _array = new int[]{ rhythm };
    _phase = phase;
  }

  public LineDashPattern(int rhythm_on, int rhythm_off, int phase) {
    _array = new int[]{ rhythm_on, rhythm_off };
    _phase = phase;
  }
}

public enum RenderingIntent {
  AbsoluteColorimetric,
  RelativeColorimetric,
  Saturation,
  Perceptual,
}

public enum PdfColorSpace {
  DeviceGray,
  DeviceRGB,
  DeviceCMYK,
  CalGray,
  CalRGB,
  Lab,
  ICCBased,
  Pattern,
  Indexed,
  Separation,
  DeviceN
}

public enum AlphaSource {
  Shape, // true
  Opacity // false
}

public static class PdfGraphicsObjectExtenstions {
  public static int Value(this LineCapStyle capStyle){
    return capStyle switch {
      LineCapStyle.SquareButt => 0,
      LineCapStyle.Round => 1,
      LineCapStyle.ProjectingSquare => 2,
      _ => 0
    };
  }
}

public class PdfGraphicsObject {

  protected List<PdfGraphicsObject> SubObjects = new List<PdfGraphicsObject>();

  // other graphic state operators need to be set with gs operator that references an ExtGState parameter dictionary (8.4.5)

  // operator cm
  PdfMatrix _currentTransformationMatrix = new PdfMatrix();
  
  // TODO add ClippingPath;

  PdfColorSpace? _colorSpace;
  List<decimal> _color;
  //TODO figure out color


  // TODO add TextState;
  
  // operator w
  decimal? _lineWidth;
  //operator J
  LineCapStyle? _lineCapStyle;
  //operator j
  LineJoinStyle? _lineJoinStyle;
  //operator M
  decimal? _miterLimit;

  //operator d
  LineDashPattern _dashPattern;
  //operator ri
  RenderingIntent? _renderingIntent;
  //TODO bool StrokeAdjustment = false;
  // TODO BlendMode BlendMode = BlendMode.Normal;
  //TODO add SoftMask;
  //TODO decimal AlphaConstant = 1.0;
  //TODO AlphaSource AlphaSource = AlphaSource.Opacity;

  public PdfGraphicsObject SetGray(decimal value) {
    _colorSpace = PdfColorSpace.DeviceGray;
    _color = new List<decimal>{ value };
    return this;
  }

  public PdfGraphicsObject SetRGB(decimal r, decimal g, decimal b) {
    _colorSpace = PdfColorSpace.DeviceRGB;
    _color = new List<decimal>{ r, g, b };
    return this;
  }

  public PdfGraphicsObject SetCMYK(decimal c, decimal m, decimal y, decimal k) {
    _colorSpace = PdfColorSpace.DeviceCMYK;
    _color = new List<decimal>{ c, m, y, k };
    return this;
  }

  public PdfGraphicsObject Translate(decimal x, decimal y) {
    _currentTransformationMatrix = _currentTransformationMatrix.Translate(x, y);
    return this;
  }

  public PdfGraphicsObject Scale(decimal x, decimal y) {
    _currentTransformationMatrix = _currentTransformationMatrix.Scale(x, y);
    return this;
  }

  public PdfGraphicsObject Rotate(decimal angle) {
    _currentTransformationMatrix = _currentTransformationMatrix.Rotate(angle);
    return this;
  }

  public PdfGraphicsObject Skew(decimal angle_a, decimal angle_b) {
    _currentTransformationMatrix = _currentTransformationMatrix.Skew(angle_a, angle_b);
    return this;
  }

  public PdfGraphicsObject LineWidth(decimal width) {
    _lineWidth = width;
    return this;
  }

  public PdfGraphicsObject CapStyle(LineCapStyle style) {
    _lineCapStyle = style;
    return this;
  }

  public PdfGraphicsObject JoinStyle(LineJoinStyle style) {
    _lineJoinStyle = style;
    return this;
  }

  public PdfGraphicsObject MiterLimit(decimal limit) {
    _miterLimit = limit;
    return this;
  }

  public PdfGraphicsObject DashPattern(LineDashPattern pattern) {
    _dashPattern = pattern;
    return this;
  }

  public PdfGraphicsObject Intent(RenderingIntent intent) {
    _renderingIntent = intent;
    return this;
  }

  // device dependent graphics state parameters
  //TODO add Overprint;
  //TODO add OverprintMode;
  //TODO add BlackGeneration;
  //TODO add UndercolorRemoval;
  //TODO add Transfer;
  //TODO add Halftone;
  //TODO add Flatness; //operator i
  //TODO add Smoothness;

  public virtual PdfExporterResults Export(Stream stream, int objectNumber) {
    if (_currentTransformationMatrix.transforms.Any()) {
      foreach(var transform in _currentTransformationMatrix.transforms) {
        stream.Write($"{transform[0]} {transform[1]} {transform[2]} {transform[3]} {transform[4]} {transform[5]} cm\n".ToUTF8());
      }
    }

    if (_lineWidth != null) stream.Write($"{_lineWidth} w\n".ToUTF8());
    if (_lineCapStyle != null) stream.Write($"{_lineCapStyle.Value.Value()} J\n".ToUTF8());

    foreach (var obj in SubObjects) {
      stream.Write("q\n".ToUTF8());
      obj.Export(stream, objectNumber);
      stream.Write("Q\n".ToUTF8());
    }

    return new PdfExporterResults();
  }
}