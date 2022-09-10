using System.Text;

namespace BlastPDF.Exporter.Util;

public static class ExporterHelperExtensions {
  public static byte[] ToUTF8(this string value) {
    return Encoding.UTF8.GetBytes(value);
  }
}