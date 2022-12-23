
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
    ImageParsingExample.Run("../../../images/bmp/w3c_home.bmp");
    //ImageParsingExample.Run("../../../images/qoi/testcard.qoi");
    //ImageParsingExample.Run("../../../images/qoi/testcard_rgba.qoi");
    //ImageParsingExample.Run("../../../images/qoi/wikipedia_008.qoi");
    /*var bytes = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbbbccccccccccccccccc");
    var encoded  = PdfFilter.RunLength.Encode(bytes);
    var decoded = PdfFilter.RunLength.Decode(encoded);
    
    Console.WriteLine($"hello????? {bytes.Count()} {encoded.Count()} {decoded.Count()}");
    foreach (var b in bytes)
    {
      Console.Write(Convert.ToChar(b));
    }
    Console.WriteLine("\nEncoded");
    foreach (var b in encoded)
    {
      Console.Write(Convert.ToChar(b));
    }
    Console.WriteLine("\nDecoded");
    foreach (var b in decoded)
    {
      Console.Write(Convert.ToChar(b));
    }*/
  }
}

