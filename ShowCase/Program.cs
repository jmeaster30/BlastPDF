using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlastPDF.Builder;
using BlastPDF.Filter;
using BlastSharp.Lists;
using BlastType;

namespace ShowCase;

public class Program {
  public static void Main(string[] args)
  {
    PdfBuilderExample.Run("megatest.pdf");
    //ImageParsingExample.Run("../../../images/qoi/dice.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim10.qoi");
    //ImageParsingExample.Run("../../../images/qoi/kodim23.qoi");
    //ImageParsingExample.Run("../../../images/bmp/w3c_home.bmp");
    //ImageParsingExample.Run("../../../images/qoi/testcard.qoi");
    //ImageParsingExample.Run("../../../images/qoi/testcard_rgba.qoi");
    //ImageParsingExample.Run("../../../images/qoi/wikipedia_008.qoi");
    //ImageParsingExample.Run("../../../images/gif/bird.gif");

    //BlastFont.Load("../../../../Samples/airtraveler.otf");
  }
}

