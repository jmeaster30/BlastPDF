namespace BlastPDF.Builder.Graphics.Drawing;

public class PdfPathMove : PdfGraphicsObject {
  public decimal X { get; }
  public decimal Y { get; }
  public PdfPathMove(decimal x, decimal y) {
    X = x; Y = y;
  }
}

public class PdfPathLine : PdfGraphicsObject {
  public decimal X;
  public decimal Y;
  public PdfPathLine(decimal x, decimal y) {
    X = x; Y = y;
  }
}

public class PdfPathBezier : PdfGraphicsObject {
  public decimal Control1X { get; }
  public decimal Control1Y { get; }
  public decimal Control2X { get; }
  public decimal Control2Y { get; }
  public decimal DestX { get; }
  public decimal DestY { get; }
  public PdfPathBezier(decimal control1_x, decimal control1_y, decimal control2_x, decimal control2_y, decimal dest_x, decimal dest_y) {
    Control1X = control1_x; Control1Y = control1_y; 
    Control2X = control2_x; Control2Y = control2_y; 
    DestX = dest_x; DestY = dest_y;
  }
}

public class PdfPathRect : PdfGraphicsObject {
  public decimal X { get; }
  public decimal Y { get; }
  public decimal Width { get; }
  public decimal Height  { get; }
  public PdfPathRect(decimal x, decimal y, decimal width, decimal height) {
    X = x; Y = y; Width = width; Height = height;
  }
}

public class PdfPathClose : PdfGraphicsObject {}

public class PdfPathPaint : PdfGraphicsObject {
  public PaintModeEnum PaintMode { get; }
  public PdfPathPaint(PaintModeEnum paintMode) {
    PaintMode = paintMode;
  }
}

public enum PaintModeEnum {
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

public static class PdfPaintModeExtensions {
  public static string getOperator(this PaintModeEnum mode) {
    return mode switch {
      PaintModeEnum.Stroke => "S",
      PaintModeEnum.CloseStroke => "s",
      PaintModeEnum.Fill => "f",
      PaintModeEnum.FillEvenOdd => "f*",
      PaintModeEnum.FillStroke => "B",
      PaintModeEnum.FillStrokeEvenOdd => "B*",
      PaintModeEnum.CloseFillStroke => "b",
      PaintModeEnum.CloseFillStrokeEvenOdd => "b*",
      PaintModeEnum.NoStroke => "n",
      _ => "n"
    };
  }
}

public static class PdfPathExtensions {
  public static PdfGraphicsObject Move(this PdfGraphicsObject graphics, decimal x, decimal y) {
    graphics.SubObjects.Add(new PdfPathMove(x, y));
    return graphics;
  }

  public static PdfGraphicsObject Line(this PdfGraphicsObject graphics, decimal x, decimal y) {
    graphics.SubObjects.Add(new PdfPathLine(x, y));
    return graphics;
  }

  public static PdfGraphicsObject Bezier(this PdfGraphicsObject graphics, decimal c1x, decimal c1y, decimal c2x, decimal c2y, decimal dx, decimal dy) {
    graphics.SubObjects.Add(new PdfPathBezier(c1x, c1y, c2x, c2y, dx, dy));
    return graphics;
  }

  public static PdfGraphicsObject Close(this PdfGraphicsObject graphics) {
    graphics.SubObjects.Add(new PdfPathClose());
    return graphics;
  }

  public static PdfGraphicsObject Rect(this PdfGraphicsObject graphics, decimal x, decimal y, decimal width, decimal height) {
    graphics.SubObjects.Add(new PdfPathRect(x, y, width, height));
    return graphics;
  }

  public static PdfGraphicsObject Paint(this PdfGraphicsObject graphics, PaintModeEnum paintOp) {
    graphics.SubObjects.Add(new PdfPathPaint(paintOp));
    return graphics;
  }
}