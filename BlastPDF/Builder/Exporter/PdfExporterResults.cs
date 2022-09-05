using System.Collections.Generic;

namespace BlastPDF.Builder.Exporter;

public class PdfExporterResults {
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