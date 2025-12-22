#:package PdfSharp@6.2.3

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;


var inputFolder = "sample";
var outputFile = "merged.pdf";
var outputFilePath = Path.Combine(inputFolder, outputFile);
var pdfFiles = Directory
    .EnumerateFiles(inputFolder, "*.pdf", SearchOption.AllDirectories)
    .Where(e => !e.EndsWith(outputFile));

using var doc = new PdfDocument();
foreach (var inputFile in pdfFiles)
{
    using var pdfInput = PdfReader.Open(inputFile, PdfDocumentOpenMode.Import);
    var fileName = Path.GetFileName(inputFile);
    System.Console.WriteLine($"Adding {fileName} (Pages: {pdfInput.PageCount})");
    foreach (var pi in pdfInput.Pages)
    {
        doc.AddPage(pi);
    }
}

doc.Save(outputFilePath);