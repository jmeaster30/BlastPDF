using BlastPDF.Builder.Graphics.Util;
using BlastPDF.Builder.Interfaces;
using System.Collections.Generic;
using System.IO;

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

//public class ColorSpace {

//}

public enum AlphaSource {
  Shape, // true
  Opacity // false
}

public class PdfGraphicsObject : IPdfStreamExporter {

  protected List<PdfGraphicsObject> SubObjects = new List<PdfGraphicsObject>();

  // other graphic state operators need to be set with gs operator that references an ExtGState parameter dictionary (8.4.5)

  // operator cm
  PdfMatrix _currentTransformationMatrix = new PdfMatrix();
  
  // TODO add ClippingPath;

  // TODO add ColorSpace;
  // TODO add Color;
  // TODO add TextState;
  
  // operator w
  decimal _lineWidth = 1.0M;
  //operator J
  LineCapStyle _lineCapStyle = LineCapStyle.SquareButt;
  //operator j
  LineJoinStyle _lineJoinStyle = LineJoinStyle.Mitered;
  //operator M
  decimal _miterLimit = 10.0M;

  //operator d
  LineDashPattern _dashPattern = LineDashPattern.Solid;
  //operator ri
  RenderingIntent _renderingIntent = RenderingIntent.RelativeColorimetric;
  //TODO bool StrokeAdjustment = false;
  // TODO BlendMode BlendMode = BlendMode.Normal;
  //TODO add SoftMask;
  //TODO decimal AlphaConstant = 1.0;
  //TODO AlphaSource AlphaSource = AlphaSource.Opacity;

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

  public virtual void Export(Stream stream) {
    // TODO
  }
}