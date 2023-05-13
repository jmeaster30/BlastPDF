
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
    var bytes = Encoding.ASCII.GetBytes(@"i did it :)");
    var encoded  = PdfFilter.ASCII85.Encode(bytes);
    var decoded = PdfFilter.ASCII85.Decode(encoded);
    
    Console.WriteLine($"bytes   '{Encoding.ASCII.GetString(bytes.ToArray())}'");
    Console.WriteLine($"encoded '{Encoding.ASCII.GetString(encoded.ToArray())}'");
    Console.WriteLine($"decoded '{Encoding.ASCII.GetString(decoded.ToArray())}'");

    var (diffOffset, leftDiff, rightDiff) = bytes.FirstDifference(decoded);
    Console.WriteLine(diffOffset == -1 ? "SUCCESS" : $"[{diffOffset}] {leftDiff} != {rightDiff}");
    
    var abytes = Encoding.ASCII.GetBytes(@"i did it :) asdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdfasdf");
    var aencoded  = PdfFilter.LZW.Encode(bytes);
    var adecoded = PdfFilter.LZW.Decode(encoded);
    
    Console.WriteLine($"bytes   '{Encoding.ASCII.GetString(abytes.ToArray())}'");
    Console.WriteLine($"encoded '{Encoding.ASCII.GetString(aencoded.ToArray())}'");
    Console.WriteLine($"decoded '{Encoding.ASCII.GetString(adecoded.ToArray())}'");

    var (adiffOffset, aleftDiff, arightDiff) = bytes.FirstDifference(adecoded);
    Console.WriteLine(diffOffset == -1 ? "SUCCESS" : $"[{adiffOffset}] {aleftDiff} != {arightDiff}");
  }
}

