using System;
using System.Collections.Generic;
using BlastPDF.Builder.Graphics;

namespace BlastPDF.Builder;

public class PdfPage {
  int dotsPerInch = 72;
  decimal width = 8.5M;
  decimal height = 11;

  public decimal CropBoxX => 0.0M; 
  public decimal CropBoxY => 0.0M;
  public decimal CropBoxW => dotsPerInch * width;
  public decimal CropBoxH => dotsPerInch * height;

  public List<PdfGraphicsObject> Objects { get; } = new();
  public Dictionary<string, PdfObject> Resources { get; } = new();

  public static PdfPage Create() { return new PdfPage(); }

  public PdfPage AddResource(string resourceName, PdfObject pdfObject) {
    if (string.IsNullOrEmpty(resourceName)) throw new ArgumentNullException("resourceName");
    if (Resources.ContainsKey(resourceName)) throw new ArgumentException($"Resource '{resourceName}' already exists as a resource for this page :(");
    Resources.Add(resourceName, pdfObject);
    return this;
  }

  public PdfPage AddGraphics(PdfGraphicsObject graphicsObject) {
    Objects.Add(graphicsObject);
    return this;
  }

  // sets UserUnit entry for page
  public PdfPage DotsPerInch(int value) { 
    dotsPerInch = value;
    return this; 
  }

  public PdfPage Width(decimal value) {
    width = value;
    return this;
  }

  public PdfPage Height(decimal value) {
    height = value;
    return this;
  }
}