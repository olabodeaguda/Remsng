using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
//using iTextSharp.text.pdf;
using RemsNG.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemsNG.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GetPdf(string htmlstring)
        {
            byte[] pdfbyte = null;
            MemoryStream memoryStream = new MemoryStream();
            var styles = new StyleSheet();

            //var props = new Hashtable
            //{
            //    { "img_provider", new MyImageFactory()}
            //};
            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            var objects = HtmlWorker.ParseToList(new StringReader(htmlstring), styles).ToArray();

            for (int i = 0; i < objects.Length; i++)
            {
                document.Add((IElement)objects[i]);
            }

            document.Close();
            pdfbyte = memoryStream.ToArray();
            memoryStream.Dispose();
            return pdfbyte;
        }

        public byte[] GetPdf(string[] htmlstrings)
        {
            byte[] pdfbyte = null;

            List<byte[]> lt = new List<byte[]>();
            foreach (var tm in htmlstrings)
            {
                lt.Add(GetPdf(tm));
            }
            pdfbyte = MergeFiles(lt);

            return pdfbyte;
        }

        private byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            Document document = new Document();
            using (MemoryStream ms = new MemoryStream())
            {
                PdfCopy copy = new PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Iterate through all pdf documents
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        documentPageCounter++;
                        PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                        PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                        //// Write header
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase("PDF Merger by Helvetic Solutions"), importedPage.Width / 2, importedPage.Height - 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);

                        // Write footer
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase(String.Format("Page {0}", documentPageCounter)), importedPage.Width / 2, 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);

                        pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }
    }
}

