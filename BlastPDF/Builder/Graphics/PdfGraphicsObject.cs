using System.Collections.Generic;

namespace BlastPDF.Builder.Graphics;

public class PdfGraphicsObject : PdfObject {

  public List<PdfGraphicsObject> SubObjects { get; protected set; }= new List<PdfGraphicsObject>();

  // other graphic state operators need to be set with gs operator that references an ExtGState parameter dictionary (8.4.5)
  // TODO add ClippingPath;

  // TODO add TextState;
  //TODO bool StrokeAdjustment = false;
  // TODO BlendMode BlendMode = BlendMode.Normal;
  //TODO add SoftMask;
  //TODO decimal AlphaConstant = 1.0;
  //TODO AlphaSource AlphaSource = AlphaSource.Opacity;

  public static PdfGraphicsObject Create() {
    return new PdfGraphicsObject();
  }


  // device dependent graphics state parameters
  //TODO add Overprint;
  //TODO add OverprintMode;
  //TODO add BlackGeneration;
  //TODO add UndercolorRemoval;
  //TODO add Transfer;
  //TODO add Halftone;
  //TODO add Flatness; //operator i
  //TODO add Smoothness;

  
}
