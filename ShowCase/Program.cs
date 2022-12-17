
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
        "Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure.");
    var result = Encoding.ASCII.GetBytes(
      @"9jqo^BlbD-BleB1DJ+*+F(f,q/0JhKF<GL>Cj@.4Gp$d7F!,L7@<6@)/0JDEF<G%<+EV:2F!,O<DJ+*.@<*K0@<6L(Df-\0Ec5e;DffZ(EZee.Bl.9pF""AGXBPCsi+DGm>@3BB/F*&OCAfu2/AKYi(DIb:@FD,*)+C]U=@3BN#EcYf8ATD3s@q?d$AftVqCh[NqF<G:8+EV:.+Cf>-FD5W8ARlolDIal(DId<j@<?3r@:F%a+D58'ATD4$Bl@l3De:,-DJs`8ARoFb/0JMK@qB4^F!,R<AKZ&-DfTqBG%G>uD.RTpAKYo'+CT/5+Cei#DII?(E,9)oF*2M7/c");
    var encoded  = bytes.ASCII85Encode().ToArray();
    
    Console.WriteLine($"hello????? {bytes.Count()} {encoded.Count()}");
    foreach (var b in bytes)
    {
      Console.Write(Convert.ToChar(b));
    }
    Console.WriteLine("\nDone");
    var a = false;
    for (var i = 0; i < encoded.Length; i++)
    {
      if (encoded[i] != result[i])
      {
        a = true;
        break;
      }
    }
    Console.WriteLine(a ? "Failed" : "YAY");
  }
}

