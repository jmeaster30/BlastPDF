
using System;
using System.Linq;
using System.Text;
using BlastPDF.Builder;

namespace ShowCase;

public class Program {
  public static void Main(string[] args) {
    //PdfBuilderExample.Run("test.pdf");
    //ImageParsingExample.Run("../../../images/qoi/dice.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim10.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim23.qoi");
    //ImageParsingExample.Run("../../../images/qoi/qoi_logo.qoi");
    //ImageParsingExample.Run("../../../images/qoi/testcard.qoi");
    //ImageParsingExample.Run("../../../images/qoi/testcard_rgba.qoi");
    //ImageParsingExample.Run("../../../images/qoi/wikipedia_008.qoi");
    var bytes = Encoding.ASCII.GetBytes(
        ")))))))))))))))))))))))))))))))))))))))))");
    var encoded  = bytes.Encode(PdfFilter.RunLengthDecode);
    
    Console.WriteLine($"hello????? {bytes.Count()} {encoded.Count()}");
    foreach (var b in bytes)
    {
      Console.Write(Convert.ToChar(b));
    }
    Console.WriteLine("\nDone");
    foreach (var b in encoded)
    {
      Console.WriteLine(Convert.ToInt32(b));
    }
  }
}

