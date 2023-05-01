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
        
        using var fs = File.Create(outputPdfName);
        PdfDocument.Create()
            .AddPage(PdfPage.Create()
                .DotsPerInch(100)
                .Width(10)
                .Height(10)
                //.AddGraphics(PdfGraphicsObject.Create()
                //    .SetCMYK(0.0M, 0.0M, 1.0M, 0.0M)
                //    .Rect(0, 0, 1000, 1000)
                //    .Paint(PaintModeEnum.CloseFillStroke))
                .AddGraphics(PdfTextObject.Create()
                    .ShowText("Holy crap Louis"))
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(250, 702)
                    .Scale(50.0M, 50.0M)
                    .InlineImage("../../../images/bmp/w3c_home.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.ASCII85}))
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(300, 702)
                    .Scale(50.0M, 50.0M)
                    .InlineImage("../../../images/bmp/w3c_home.bmp", FileFormat.BMP)) // ASCIIHex
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(350, 702)
                    .Scale(50.0M, 50.0M)
                    .InlineImage("../../../images/bmp/w3c_home.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.LZW}))
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(400, 702)
                    .Scale(50.0M, 50.0M)
                    .InlineImage("../../../images/bmp/w3c_home.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.ASCII85, PdfFilter.LZW}))
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(450, 702)
                    .Scale(100.0M, 100.0M)
                    .InlineImage("../../../images/bmp/cat.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.ASCIIHex, PdfFilter.LZW}))
                ).Save(fs);
    }
}
