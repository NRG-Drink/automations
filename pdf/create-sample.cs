#:package PdfSharp@6.2.3

using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;

GlobalFontSettings.UseWindowsFontsUnderWindows = true;
var outputFolder = "sample";

for (var i = 0; i < 3; i++)
{
    var outputFile = Path.Combine(outputFolder, $"sample_{i}.pdf");
    using var doc = GeneratePdfWithCenteredText($"Document {i}: Page 1");
    doc.Save(outputFile);
}

PdfDocument GeneratePdfWithCenteredText(string text)
{
    var doc = new PdfDocument();

    var pageInput = new PdfPage()
    {
        Orientation = PageOrientation.Portrait,
        Size = PageSize.A4
    };
    var page = doc.AddPage(pageInput);

    var gfx = XGraphics.FromPdfPage(page);
    var font = new XFont("Times New Roman", 20, XFontStyleEx.BoldItalic);

    gfx.DrawString(
        text,
        font,
        XBrushes.Black,
        new XRect(0, 0, page.Width.Value, page.Height.Value),
        XStringFormats.Center);

    return doc;
}