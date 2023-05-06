using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlastPDF.Internal.Helpers
{
  public static class HelperExtensions
  {
    public static string ConcatByte(this string lexeme, int value)
    {
      return $"{lexeme}{Encoding.UTF8.GetString(new[] { (byte)value })}";
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
      var idx = 0;
      while (idx < str.Length)
      {
        var window = str[idx..(idx + size)];
        sb.Append(func(window));
        idx += size;
      }
      return sb.ToString();
    }

    public static int IndexOf(this string str, Func<char, bool> func)
    {
      return str.TakeWhile(c => !func(c)).Count();
    }

    public static List<List<T>> Split<T>(this List<T> values, Func<T, T, bool> func, bool inclusive = false)
    {
      var results = new List<List<T>>();
      var current = new List<T>();
      T prev = default(T);
      foreach (var value in values) {
        if (func(prev, value)) {
          if (inclusive) {
            current.Add(value);
          }
          results.Add(current);
          current = new List<T>();
          prev = value;
          continue;
        }
        current.Add(value);
        prev = value;
      }
      return results;
    }

    //public static string ToLiteral(this string valueTextForCompiler)
    //{
    //   return Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(valueTextForCompiler, false);
    //}
  }
}