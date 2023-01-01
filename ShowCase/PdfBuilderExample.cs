using System;
using System.IO;
using BlastIMG;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using BlastPDF.Exporter.Basic;

namespace ShowCase;

public class PdfBuilderExample
{
    public static void Run(string outputPdfName)
    {
        if (File.Exists(outputPdfName)) {
            File.Delete(outputPdfName);
        }
        
        using FileStream fs = File.Create(outputPdfName);
        PdfDocument.Create()
            .AddPage(PdfPage.Create()
                .DotsPerInch(100)
                .Width(10)
                .Height(10)
                .AddGraphics(PdfGraphicsObject.Create()
                    .SetCMYK(0.0M, 0.0M, 1.0M, 0.0M)
                    .Rect(0, 0, 1000, 1000)
                    .Paint(PaintModeEnum.CloseFillStroke))
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(250, 702)
                    .Scale(50.0M, 50.0M)
                    .InlineImage("../../../images/bmp/w3c_home.bmp", FileFormat.BMP)))
            .Save(fs);
    }
}
