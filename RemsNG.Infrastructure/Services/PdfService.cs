﻿using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
//using iTextSharp.text.pdf;
using RemsNG.Common.Interfaces.Services;
using RemsNG.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace RemsNG.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly TemplateDetail _template;

        public PdfService(TemplateDetail templateDetail)
        {
            _template = templateDetail;
        }

        public byte[] GetBytes(string[] htmlStrings)
        {
            byte[] pdfbyte = null;

            List<byte[]> lt = new List<byte[]>();
            foreach (var tm in htmlStrings)
            {
                lt.Add(GetBytes(tm));
            }
            pdfbyte = MergeFiles(lt);

            return pdfbyte;
        }

        public byte[] GetBytes(string[] htmlStrings, TemplateType templateType)
        {
            byte[] pdfbyte = null;

            List<byte[]> lt = new List<byte[]>();
            foreach (var tm in htmlStrings)
            {
                lt.Add(GetBytes(tm, templateType));
            }
            pdfbyte = MergeFiles(lt);

            return pdfbyte;
        }

        private byte[] GetBytes(string htmlString)
        {
            byte[] pdfbyte = null;
            MemoryStream memoryStream = new MemoryStream();
            var styles = new StyleSheet();

            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            var objects = HtmlWorker.ParseToList(new StringReader(htmlString), styles).ToArray();

            for (int i = 0; i < objects.Length; i++)
            {
                document.Add((IElement)objects[i]);
            }

            document.Close();
            pdfbyte = memoryStream.ToArray();
            memoryStream.Dispose();
            return pdfbyte;
        }

        private byte[] GetBytes(string htmlString, TemplateType templateType)
        {
            byte[] pdfbyte = null;
            MemoryStream memoryStream = new MemoryStream();
            var styles = new StyleSheet();

            var document = new Document(PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();
            if (templateType == TemplateType.DemandNotice)
            {
                if (!string.IsNullOrEmpty(_template.LcdaLogo))
                {
                    Image lcda = Image.GetInstance(_template.LcdaLogo);
                    lcda.SetAbsolutePosition(20, 717);
                    lcda.ScaleAbsoluteHeight(120);
                    lcda.ScaleAbsoluteWidth(120);

                    document.Add(lcda);
                }

                if (!string.IsNullOrEmpty(_template.LagosLogo))
                {
                    Image lag = Image.GetInstance(_template.LagosLogo);
                    lag.SetAbsolutePosition(460, 717);
                    lag.ScaleAbsoluteHeight(120);
                    lag.ScaleAbsoluteWidth(120);
                    document.Add(lag);
                }
            }
            else if (templateType == TemplateType.Reminder)
            {
                if (!string.IsNullOrEmpty(_template.ReminderLcdaLogo))
                {
                    Image lcda = Image.GetInstance(_template.ReminderLcdaLogo);
                    lcda.SetAbsolutePosition(20, 717);
                    lcda.ScaleAbsoluteHeight(120);
                    lcda.ScaleAbsoluteWidth(120);

                    document.Add(lcda);
                }

                if (!string.IsNullOrEmpty(_template.ReminderLagosLogo))
                {
                    Image lag = Image.GetInstance(_template.ReminderLagosLogo);
                    lag.SetAbsolutePosition(460, 717);
                    lag.ScaleAbsoluteHeight(120);
                    lag.ScaleAbsoluteWidth(120);
                    document.Add(lag);
                }

                if (!string.IsNullOrEmpty(_template.ReminderBackgroundLogo))
                {
                    Image bkgrd = Image.GetInstance(_template.ReminderBackgroundLogo);
                    bkgrd.SetAbsolutePosition(250, 350);
                    bkgrd.ScaleAbsoluteHeight(200);
                    bkgrd.ScaleAbsoluteWidth(200);
                    document.Add(bkgrd);
                }
            }

            var objects = HtmlWorker.ParseToList(new StringReader(htmlString), styles).ToArray();



            for (int i = 0; i < objects.Length; i++)
            {
                document.Add((IElement)objects[i]);
            }

            document.Close();
            pdfbyte = memoryStream.ToArray();
            memoryStream.Dispose();
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

