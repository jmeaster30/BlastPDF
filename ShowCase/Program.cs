
namespace ShowCase;

public class Program {
  public static void Main(string[] args) {
    //PdfBuilderExample.Run("test.pdf");
    ImageParsingExample.Run("../../../images/qoi/dice.qoi");
    ImageParsingExample.Run("../../../images/qoi/kodim10.qoi");
    ImageParsingExample.Run("../../../images/qoi/kodim23.qoi");
    ImageParsingExample.Run("../../../images/qoi/qoi_logo.qoi");
    ImageParsingExample.Run("../../../images/qoi/testcard.qoi");
    ImageParsingExample.Run("../../../images/qoi/testcard_rgba.qoi");
    ImageParsingExample.Run("../../../images/qoi/wikipedia_008.qoi");
  }
}

