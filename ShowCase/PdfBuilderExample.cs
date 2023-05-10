using System;
using System.IO;
using BlastIMG;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using BlastPDF.Exporter.Basic;
using BlastSharp.Dates;
using Microsoft.VisualBasic;

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
            .AddMetadata(new ()
            {
                {"Creator", new PdfStringValue("John Easterday")},
                {"Producer", new PdfStringValue("BlastPDF")},
                {"Title", new PdfStringValue("Super Incredible PDF >:)")},
                {"CreationDate", new PdfDateValue(BlastDateTime.Year(1998).June().Day(30))}
            })
            .AddPage(PdfPage.Create()
                .DotsPerInch(100)
                .Width(10)
                .Height(10)
                //.AddGraphics(PdfGraphicsObject.Create()
                //    .SetCMYK(0.0M, 0.0M, 1.0M, 0.0M)
                //    .Rect(0, 0, 1000, 1000)
                //    .Paint(PaintModeEnum.CloseFillStroke))
                .AddGraphics(PdfTextObject.Create()
                    .TextLeading(12)
                    .SetFont("F1", 24)
                    .NextLineOffset(100, 100)
                    .ShowText("have we gone too far")
                    .NextLineOffset(156, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far"))
                /*.AddGraphics(PdfGraphicsObject.Create()
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
                */
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(200, 200)
                    .Scale(600.0M, 600.0M)
                    .InlineImage("../../../images/bmp/cat.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.ASCIIHex, PdfFilter.LZW}))
                ).Save(fs);

        //TestTemplate.OHYEAH();
    }
}
