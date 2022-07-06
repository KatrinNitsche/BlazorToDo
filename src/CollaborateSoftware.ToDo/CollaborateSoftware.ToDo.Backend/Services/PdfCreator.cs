using OpenHtmlToPdf;
using System.IO;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class PdfCreator : IPdfCreator
    {
        public async Task<bool> CreateDailySheet()
        {
            try
            {
                var filePath = @"C:\tmp\daily.pdf";
                var html = GetHtmlCode();

                var pdf = Pdf
                    .From(html)
                    .OfSize(PaperSize.A4)
                    .WithTitle("Title")
                    .WithoutOutline()
                    .WithMargins(1.25.Centimeters())
                    .Portrait()
                    .Comressed()
                    .Content();

                await File.WriteAllBytesAsync(filePath, pdf);

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private string GetHtmlCode()
        {
            var htmlCode = string.Empty;

            htmlCode = "<!DOCTYPE html>" +
                "<html>" +
                "<head><meta charset='UTF-8'><title>Title</title></head>" +
                "<body>Body text...</body>" +
                "</html>";

            return htmlCode;
        }
    }
}
