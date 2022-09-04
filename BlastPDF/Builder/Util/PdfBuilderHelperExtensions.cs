using System.Text;

namespace BlastPDF.Builder.Util;

public static class PdfBuilderHelperExtensions {
  public static byte[] ToUTF8(this string value) {
    return Encoding.UTF8.GetBytes(value);
  }
}