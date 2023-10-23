using System.Linq;

namespace BlastPDF.Builder;

public static class Util
{
    public static string EscapedString(this string input)
    {
        return input.Aggregate("", (current, c) => current + c switch
        {
            '\\' => "\\\\",
            '"' => "\\\"",
            '\'' => "\\'",
            (char)0 => "\\0",
            '\a' => "\\a",
            '\b' => "\\b",
            '\f' => "\\f",
            '\n' => "\\n",
            '\r' => "\\r",
            '\t' => "\\t",
            '\v' => "\\v",
            '(' => "\\(",
            ')' => "\\)",
            _ => $"{c}"
        });
    }
}