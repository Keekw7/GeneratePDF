using HandlebarsDotNet;
using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System.Drawing;
using System.IO;

namespace GeneratePDF.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratePDF : ControllerBase
    {
        [HttpGet]
        public byte[] GetPdf()
        {
            string footerTemplateHtmlString = System.IO.File.ReadAllText(@$"Files\footer.html");

            string templateHtmlString = System.IO.File.ReadAllText(@$"Files\template.html");

            var converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginTop = -5;
            converter.Options.MarginLeft = -5;

            converter.Options.DisplayHeader = true;
            converter.Header.Height = 65;

            converter.Options.DisplayFooter = true;
            var footerHtml = new PdfHtmlSection(footerTemplateHtmlString, null);
            converter.Footer.Add(footerHtml);
            converter.Footer.Height = 105;

            var vdwLogo = Image.FromFile(@$"Files\vandewiele-logo.png");
            var rojLogo = Image.FromFile(@$"Files\roj-logo.jpg");

            using var ms = new MemoryStream();

            var pdf = converter.ConvertHtmlString(templateHtmlString);
            CreateImageElement(vdwLogo, pdf, true);
            CreateImageElement(rojLogo, pdf, false);

            pdf.Save(ms);
            ms.Position = 0;

            return ms.ToArray();
        }

        private void CreateImageElement(Image logoImg, PdfDocument pdf, bool isLogo)
        {
            var x = isLogo ? 0 : 445;
            var y = isLogo ? (-65) :(-35);
            var width = isLogo ? 168 : 110;
            var height = isLogo ? 235 : 40;
            var logoTemplate = pdf.AddTemplate(new RectangleF(x, y, width, height));
            var logoImageElement = new PdfImageElement(0, 0, width, height, logoImg);
            logoTemplate.Add(logoImageElement);
        }
    }
}