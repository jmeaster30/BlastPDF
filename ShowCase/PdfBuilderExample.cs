using System;
using System.Collections.Generic;
using System.IO;
using BlastPDF.Builder;
using BlastPDF.Builder.Graphics;
using BlastPDF.Builder.Graphics.Drawing;
using BlastPDF.Builder.Resources;
using BlastPDF.Builder.Resources.Font;
using BlastPDF.Builder.Resources.Image;
using BlastPDF.Exporter.Basic;
using BlastPDF.Filter;
using BlastSharp.Dates;
using SharperImage.Formats;

namespace ShowCase;

public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class PdfBuilderExample
{
    public static void Run(string outputPdfName)
    {
        if (File.Exists(outputPdfName)) {
            File.Delete(outputPdfName);
        }
        
        using var fs = File.Create(outputPdfName);
        PdfDocument.Create()
            .AddMetadata(new Dictionary<string, IPdfValue>
            {
                {"Creator", new PdfStringValue("John Easterday")},
                {"Producer", new PdfStringValue("BlastPDF")},
                {"Title", new PdfStringValue("Super Incredible PDF >:)")},
                {"CreationDate", new PdfDateValue(DateTime.Now.Year(1998).June().Day(30))}
            })
            .AddPage(PdfPage.Create()
                .DotsPerInch(100)
                .Width(10)
                .Height(10)
                .UseHelvetica()
                .UseCourierBold()
                .UseTimesNewRomanItalic()
                .UseImage("Cat", "../../../images/bmp/cat.bmp", FileFormat.BMP, PdfColorSpace.DeviceRGB, new []{PdfFilter.AsciiHex, PdfFilter.Lzw})
                .AddGraphics(PdfTextObject.Create()
                    .TextLeading(12)
                    .SetFont("Helvetica", 24)
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
                    .SetFont("CourierBold", 24)
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .NextLineOffset(0, -6)
                    .ShowText("too far")
                    .SetFont("TimesNewRomanItalic", 24)
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
                .AddGraphics(PdfGraphicsObject.Create()
                    .Translate(200, 200)
                    .Scale(600.0M, 600.0M)
                    .Image("Cat")
                    .ResetState()
                    .Translate(250, 250)
                    .Scale(500.0M, 500.0M)
                    .Image("Cat")
                    .ResetState()
                    .Translate(300, 300)
                    .Scale(400.0M, 400.0M)
                    .Image("Cat")
                    .ResetState()
                    .Translate(350, 350)
                    .Scale(300.0M, 300.0M)
                    .Image("Cat")
                    .ResetState()
                    .Translate(400, 400)
                    .Scale(200.0M, 200.0M)
                    .Image("Cat"))
                ).Save(fs);

        //if (File.Exists("template_test.pdf")) {
        //    File.Delete("template_test.pdf");
        //}
        
        //using var templateFile = File.Create("template_test.pdf");
        
        //var template = new TestLayout {
        //    Author = new Person
        //    {
        //        FirstName = "John",
        //        LastName = "Easterday"
        //    }
        //};
        
        //template.Save(templateFile);

        var date = DateTime.Now.Year(1998).June().Day(30);
        Console.WriteLine(date.ToLocalTime().ToLongTimeString());
        Console.WriteLine(new PdfDateValue(date.ToLocalTime()).ToString());
    }
}
