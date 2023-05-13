
using System;
using System.Linq;
using System.Text;
using BlastPDF.Builder;
using BlastPDF.Filter;
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
    var encoded  = PdfFilter.Ascii85.Encode(bytes);
    var decoded = PdfFilter.Ascii85.Decode(encoded);
    
    Console.WriteLine($"bytes   '{Encoding.ASCII.GetString(bytes.ToArray())}'");
    Console.WriteLine($"encoded '{Encoding.ASCII.GetString(encoded.ToArray())}'");
    Console.WriteLine($"decoded '{Encoding.ASCII.GetString(decoded.ToArray())}'");

    var (diffOffset, leftDiff, rightDiff) = bytes.FirstDifference(decoded);
    Console.WriteLine(diffOffset == -1 ? "SUCCESS" : $"[{diffOffset}] {leftDiff} != {rightDiff}");
        
    var abytes = Encoding.ASCII.GetBytes(@"Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure.");
    var aencoded  = PdfFilter.Ascii85.Encode(abytes);
    var adecoded = PdfFilter.Ascii85.Decode(aencoded);
    
    Console.WriteLine($"bytes   '{Encoding.ASCII.GetString(abytes.ToArray())}'");
    Console.WriteLine($"encoded '{Encoding.ASCII.GetString(aencoded.ToArray())}'");
    Console.WriteLine($"decoded '{Encoding.ASCII.GetString(adecoded.ToArray())}'");

    var (adiffOffset, aleftDiff, arightDiff) = abytes.FirstDifference(adecoded);
    Console.WriteLine(adiffOffset == -1 ? "SUCCESS" : $"[{adiffOffset}] {aleftDiff} != {arightDiff}");
  }
}

