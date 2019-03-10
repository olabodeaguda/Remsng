using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using RemsNG.Common.Interfaces.Services;
using System.IO;

namespace RemsNG.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GetPdf(string htmlstring)
        {
            byte[] pdfbyte = null;
            MemoryStream memoryStream = new MemoryStream();

            // create a StyleSheet
            var styles = new StyleSheet();
            styles.LoadTagStyle("tr", "height", "30");

            //var props = new Hashtable
            //{
            //    { "img_provider", new MyImageFactory()}
            //};

            // step 1
            var document = new Document();
            // step 2
            PdfWriter.GetInstance(document, memoryStream);
            // step 3
            // document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            //var objects = HtmlWorker.ParseToList(
            //    new StringReader(htmlstring),
            //    styles,
            //    props
            //);

            var objects = HtmlWorker.ParseToList(new StringReader(htmlstring), styles);

            foreach (IElement element in objects)
            {
                document.Add(element);
            }

            document.Close();
            pdfbyte = memoryStream.ToArray();
            memoryStream.Dispose();
            return pdfbyte;
        }

        public byte[] GetPdf(string[] htmlstrings)
        {
            throw new System.NotImplementedException();
        }
    }
}
