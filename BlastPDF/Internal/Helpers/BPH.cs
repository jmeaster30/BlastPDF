using System.Text;

namespace BlastPDF.Internal.Helpers;

public class BPH
{
    public static string ByteNumToString(int b)
    {
        return Encoding.UTF8.GetString(new[] { (byte)b });
    }
}