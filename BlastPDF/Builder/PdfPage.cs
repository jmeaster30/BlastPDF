using System.Collections.Generic;
using System.Linq;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Exporter;
using System.IO;
using BlastPDF.Builder.Util;

namespace BlastPDF.Builder;

public class PdfPage {

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

  public PdfExporterResults Export(Stream stream, int objectNumber) {
    var results = new PdfExporterResults();

    var crossReferences = new List<(int, long)>();
    var contentRefs = new List<int>();

    var nextStart = objectNumber;
    foreach(var obj in graphicsObjects) {
      contentRefs.Add(nextStart);
      crossReferences.Add((nextStart, stream.Position));
      stream.Write($"{nextStart} 0 obj\n".ToUTF8());
      stream.Write("<<\n".ToUTF8());
      stream.Write($"/Length {nextStart + 1} 0 R\n".ToUTF8());
      stream.Write(">>\n".ToUTF8());
      stream.Write("stream\n".ToUTF8());

      var startOffset = stream.Position;
      var refs = obj.Export(stream, nextStart + 2);
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
    stream.Write($"/MediaBox [ {cropBoxOriginX} {cropBoxOriginY} {cropBoxWidth} {cropBoxHeight} ]\n".ToUTF8());
    stream.Write("/Contents [\n".ToUTF8());
    foreach(var contentRef in contentRefs) {
      stream.Write($"{contentRef} 0 R\n".ToUTF8());
    }
    stream.Write("]\n".ToUTF8());
    stream.Write(">>\n".ToUTF8());
    stream.Write("endobj\n\n".ToUTF8());

    return results;
  }

}