using System;
using System.Collections.Generic;

namespace BlastPDF.Builder.Graphics.Util;

public class PdfMatrix {

  List<decimal[]> transforms = new List<decimal[]>();

  public PdfMatrix Translate(decimal x, decimal y) {
    transforms.Add(new decimal[] {1.0M, 0.0M, 0.0M, 1.0M, x, y});
    return this;
  }

  public PdfMatrix Scale(decimal x, decimal y) {
    transforms.Add(new decimal[] {x, 0.0M, 0.0M, y, 0.0M, 0.0M});
    return this;
  }

  public PdfMatrix Rotate(decimal angle) {
    transforms.Add(new decimal[] {(decimal)Math.Cos((double)angle), (decimal)Math.Sin((double)angle), -(decimal)Math.Sin((double)angle), (decimal)Math.Cos((double)angle), 0.0M, 0.0M});
    return this;
  }

  public PdfMatrix Skew(decimal angle_a, decimal angle_b) {
    transforms.Add(new decimal[] {1.0M, (decimal)Math.Tan((double)angle_a), (decimal)Math.Tan((double)angle_b), 1.0M, 0.0M, 0.0M});
    return this;
  }

}