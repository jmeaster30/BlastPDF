using System.IO;

namespace BlastPDF.Builder.Interfaces;

public interface IPdfStreamExporter {
  void Export(Stream stream);
}