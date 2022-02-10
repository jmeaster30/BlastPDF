using System.Text;

namespace BlastPDF.Internal.Helpers
{
  public static class HelperExtensions
  {
    public static string ConcatByte(this string str, int b)
    {
      return str + Encoding.UTF8.GetString(new[] { (byte)b });
    }
  }
}