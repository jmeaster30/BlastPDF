
using System;
using System.Linq;
using System.Text;
using BlastPDF.Builder;
using BlastSharp.Lists;

namespace ShowCase;

public class Program {
  public static void Main(string[] args) {
    PdfBuilderExample.Run("test.pdf");
    //ImageParsingExample.Run("../../../images/qoi/dice.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim10.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim23.qoi");
    //ImageParsingExample.Run("../../../images/bmp/w3c_home.bmp");
    //ImageParsingExample.Run("../../../images/qoi/testcard.qoi");
    //ImageParsingExample.Run("../../../images/qoi/testcard_rgba.qoi");
    //ImageParsingExample.Run("../../../images/qoi/wikipedia_008.qoi");
    //var bytes = Encoding.ASCII.GetBytes(@"i did it :)");
    //var encoded  = PdfFilter.LZW.Encode(bytes);
    //var decoded = PdfFilter.LZW.Decode(encoded);

    //var (diffOffset, leftDiff, rightDiff) = bytes.FirstDifference(decoded);
    //Console.WriteLine(diffOffset == -1 ? "SUCCESS" : $"[{diffOffset}] {leftDiff} != {rightDiff}");
  }
}

