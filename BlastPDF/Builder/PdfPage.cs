using System.Collections.Generic;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Interfaces;
using System.IO;

namespace BlastPDF.Builder;

public class PdfPage : IPdfStreamExporter {

  PdfDocument Document { get; set; }

  // DPI
  int dotsPerInch = 72;

  decimal width = 8.5M;
  decimal height = 11;
  
  // CropBox
  decimal cropBoxOriginX = 0;
  decimal cropBoxOriginY = 0;
  decimal cropBoxWidth => dotsPerInch * width;
  decimal cropBoxHeight => dotsPerInch * height;

  List<PdfGraphicsObject> graphicsObjects = new List<PdfGraphicsObject>();

  public static PdfPage Create() { return new PdfPage(); }

  public PdfPage SetDocument(PdfDocument document) {
    Document = document;
    return this;
  }

  public PdfPage AddGraphics(PdfGraphicsObject graphicsObject) {
    graphicsObjects.Add(graphicsObject);
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

  public void Export(Stream stream) {

  }

}