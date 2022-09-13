using System;
using System.Collections.Generic;

namespace BlastPDF.Builder.Graphics;

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

public class PdfColor : PdfGraphicsObject {
  public PdfColorSpace ColorSpace { get; private set; }
  public List<decimal> NonStrokeColor { get; private set; } = new List<decimal>();
  public List<decimal> StrokeColor { get; private set; } = new List<decimal>();
  
  public static PdfColor DeviceGray(decimal value) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceGray;
    result.NonStrokeColor = new List<decimal>{ value };
    result.StrokeColor = new List<decimal>();
    return result;
  }

  public static PdfColor DeviceRGB(decimal r, decimal g, decimal b) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceRGB;
    result.NonStrokeColor = new List<decimal>{ r, g, b };
    result.StrokeColor = new List<decimal>();
    return result;
  }

  public static PdfColor DeviceCMYK(decimal c, decimal m, decimal y, decimal k) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceCMYK;
    result.NonStrokeColor = new List<decimal>{ c, m, y, k };
    result.StrokeColor = new List<decimal>();
    return result;
  }

  public static PdfColor StrokeDeviceGray(decimal value) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceGray;
    result.NonStrokeColor = new List<decimal>();
    result.StrokeColor = new List<decimal>{ value };
    return result;
  }

  public static PdfColor StrokeDeviceRGB(decimal r, decimal g, decimal b) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceRGB;
    result.NonStrokeColor = new List<decimal>();
    result.StrokeColor = new List<decimal>{ r, g, b };
    return result;
  }

  public static PdfColor StrokeDeviceCMYK(decimal c, decimal m, decimal y, decimal k) {
    var result = new PdfColor();
    result.ColorSpace = PdfColorSpace.DeviceCMYK;
    result.NonStrokeColor = new List<decimal>();
    result.StrokeColor = new List<decimal>{ c, m, y, k };
    return result;
  }
}

public class PdfTransform : PdfGraphicsObject {
  public decimal[] Value { get; private set; }

  public static PdfTransform Translate(decimal x, decimal y) {
    var result = new PdfTransform();
    result.Value = new decimal[] {1.0M, 0.0M, 0.0M, 1.0M, x, y};
    return result;
  }

  public static PdfTransform Scale(decimal x, decimal y) {
    var result = new PdfTransform();
    result.Value = new decimal[] {x, 0.0M, 0.0M, y, 0.0M, 0.0M};
    return result;
  }

  public static PdfTransform Rotate(decimal angle) {
    var result = new PdfTransform();
    result.Value = new decimal[] {(decimal)Math.Cos((double)angle), (decimal)Math.Sin((double)angle), -(decimal)Math.Sin((double)angle), (decimal)Math.Cos((double)angle), 0.0M, 0.0M};
    return result;
  }

  public static PdfTransform Skew(decimal angle_a, decimal angle_b) {
    var result = new PdfTransform();
    result.Value = new decimal[] {1.0M, (decimal)Math.Tan((double)angle_a), (decimal)Math.Tan((double)angle_b), 1.0M, 0.0M, 0.0M};
    return result;
  }
}

public class PdfLineWidth : PdfGraphicsObject {
  public decimal Width { get; private set; }
  public PdfLineWidth(decimal width) {
    Width = width;
  }
}

public enum LineCapStyle {
  SquareButt,
  Round,
  ProjectingSquare
}

public class PdfLineCapStyle : PdfGraphicsObject {
  public LineCapStyle LineCapStyle { get; private set; }
  public PdfLineCapStyle(LineCapStyle style) {
    LineCapStyle = style;
  }
}

public enum LineJoinStyle {
  Mitered,
  Round,
  Bevel
}

public class PdfLineJoinStyle : PdfGraphicsObject {
  public LineJoinStyle LineJoinStyle { get; private set; }
  public PdfLineJoinStyle(LineJoinStyle style) {
    LineJoinStyle = style;
  }
}

public class PdfMiterLimit : PdfGraphicsObject {
  public decimal Limit { get; private set; }
  public PdfMiterLimit(decimal limit) {
    Limit = limit;
  }
}

public class PdfLineDashPattern : PdfGraphicsObject {
  public int[] Array { get; private set; }
  public int Phase { get; private set; }

  public static PdfLineDashPattern Solid => new PdfLineDashPattern();

  public PdfLineDashPattern() {}

  public PdfLineDashPattern(int rhythm, int phase) {
    Array = new int[]{ rhythm };
    Phase = phase;
  }

  public PdfLineDashPattern(int rhythm_on, int rhythm_off, int phase) {
    Array = new int[]{ rhythm_on, rhythm_off };
    Phase = phase;
  }
}

public enum RenderingIntent {
  AbsoluteColorimetric,
  RelativeColorimetric,
  Saturation,
  Perceptual,
}

public class PdfRenderingIntent : PdfGraphicsObject {
  public RenderingIntent RenderingIntent { get; private set; }
  public PdfRenderingIntent(RenderingIntent renderingIntent) {
    RenderingIntent = renderingIntent;
  }
}

public class PdfResetGraphicsState : PdfGraphicsObject {}

public enum AlphaSource {
  Shape, // true
  Opacity // false
}

public static class PdfGraphicsObjectExtensions {
  public static int Value(this LineCapStyle capStyle){
    return capStyle switch {
      LineCapStyle.SquareButt => 0,
      LineCapStyle.Round => 1,
      LineCapStyle.ProjectingSquare => 2,
      _ => 0
    };
  }
}

public static class PdfGraphicsStateExtensions {
  public static PdfGraphicsObject SetGray(this PdfGraphicsObject graphics, decimal value) {
    graphics.SubObjects.Add(PdfColor.DeviceGray(value));
    return graphics;
  }

  public static PdfGraphicsObject SetRGB(this PdfGraphicsObject graphics, decimal r, decimal g, decimal b) {
    graphics.SubObjects.Add(PdfColor.DeviceRGB(r, g, b));
    return graphics;
  }

  public static PdfGraphicsObject SetCMYK(this PdfGraphicsObject graphics, decimal c, decimal m, decimal y, decimal k) {
    graphics.SubObjects.Add(PdfColor.DeviceCMYK(c, m, y, k));
    return graphics;
  }

  public static PdfGraphicsObject SetStrokeGray(this PdfGraphicsObject graphics, decimal value) {
    graphics.SubObjects.Add(PdfColor.StrokeDeviceGray(value));
    return graphics;
  }

  public static PdfGraphicsObject SetStrokeRGB(this PdfGraphicsObject graphics, decimal r, decimal g, decimal b) {
    graphics.SubObjects.Add(PdfColor.StrokeDeviceRGB(r, g, b));
    return graphics;
  }

  public static PdfGraphicsObject SetStrokeCMYK(this PdfGraphicsObject graphics, decimal c, decimal m, decimal y, decimal k) {
    graphics.SubObjects.Add(PdfColor.StrokeDeviceCMYK(c, m, y, k));
    return graphics;
  }

  public static PdfGraphicsObject Translate(this PdfGraphicsObject graphics, decimal x, decimal y) {
    graphics.SubObjects.Add(PdfTransform.Translate(x, y));
    return graphics;
  }

  public static PdfGraphicsObject Scale(this PdfGraphicsObject graphics, decimal x, decimal y) {
    graphics.SubObjects.Add(PdfTransform.Scale(x, y));
    return graphics;
  }

  public static PdfGraphicsObject Rotate(this PdfGraphicsObject graphics, decimal angle) {
    graphics.SubObjects.Add(PdfTransform.Rotate(angle));
    return graphics;
  }

  public static PdfGraphicsObject Skew(this PdfGraphicsObject graphics, decimal angle_a, decimal angle_b) {
    graphics.SubObjects.Add(PdfTransform.Skew(angle_a, angle_b));
    return graphics;
  }

  public static PdfGraphicsObject LineWidth(this PdfGraphicsObject graphics, decimal width) {
    graphics.SubObjects.Add(new PdfLineWidth(width));
    return graphics;
  }

  public static PdfGraphicsObject CapStyle(this PdfGraphicsObject graphics, LineCapStyle style) {
    graphics.SubObjects.Add(new PdfLineCapStyle(style));
    return graphics;
  }

  public static PdfGraphicsObject JoinStyle(this PdfGraphicsObject graphics, LineJoinStyle style) {
    graphics.SubObjects.Add(new PdfLineJoinStyle(style));
    return graphics;
  }

  public static PdfGraphicsObject MiterLimit(this PdfGraphicsObject graphics, decimal limit) {
    graphics.SubObjects.Add(new PdfMiterLimit(limit));
    return graphics;
  }

  public static PdfGraphicsObject DashPattern(this PdfGraphicsObject graphics, PdfLineDashPattern pattern) {
    graphics.SubObjects.Add(pattern);
    return graphics;
  }

  public static PdfGraphicsObject Intent(this PdfGraphicsObject graphics, RenderingIntent intent) {
    graphics.SubObjects.Add(new PdfRenderingIntent(intent));
    return graphics;
  }

  public static PdfGraphicsObject ResetState(this PdfGraphicsObject graphics) {
    graphics.SubObjects.Add(new PdfResetGraphicsState());
    return graphics;
  }
}