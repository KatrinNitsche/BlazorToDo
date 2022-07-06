using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class PdfCreator : IPdfCreator
    {
        public async Task<bool> CreateDailySheet(List<ToDoListEntry> todos)
        {
            try
            {
                var filePath = @"C:\tmp\daily.pdf";
                var html = GetHtmlCode(todos);

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
            catch (System.Exception ex)
            {
                return false;
            }
        }

        private string GetHtmlCode(List<ToDoListEntry> todos)
        {
            var htmlCode = string.Empty;

            htmlCode = Properties.Resources.Daily;
            htmlCode = htmlCode.Replace("{plan-date}", DateTime.Now.Date.ToShortDateString());

            for (int i = 0; i < 9; i++)
            {
                if (i < todos.Count)
                {
                    htmlCode = htmlCode.Replace("{todo-text-" + (i + 1) + "}", todos[i].Title);
                }
                else
                {
                    htmlCode = htmlCode.Replace("{todo-text-" + (i + 1) + "}", string.Empty);
                }
                
            }
         

            return htmlCode;
        }
    }
}
