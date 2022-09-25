using System;
using System.Text;

namespace BlastPDF.Exporter.Util;

public static class ExporterHelperExtensions {
  public static ReadOnlySpan<byte> ToUTF8(this string value) {
    return Encoding.UTF8.GetBytes(value);
  }
}