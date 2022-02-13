using System;
using System.Text;

namespace BlastPDF.Internal.Helpers
{
  public static class HelperExtensions
  {
    public static string ConcatByte(this string str, int b)
    {
      return str + Encoding.UTF8.GetString(new[] { (byte)b });
    }

    public static string Filter(this string str, Func<char, bool> filterFunc)
    {
      var sb = new StringBuilder();
      foreach (var c in str)
      {
        sb.Append(filterFunc(c) ? c.ToString() : "");
      }
      return sb.ToString();
    }

    public static string Window(this string str, int size, Func<string, string> func)
    {
      var sb = new StringBuilder();
      int idx = 0;
      while (idx < str.Length)
      {
        var window = str[idx..(idx + size)];
        sb.Append(func(window));
        idx += size;
      }
      return sb.ToString();
    }
  }
}