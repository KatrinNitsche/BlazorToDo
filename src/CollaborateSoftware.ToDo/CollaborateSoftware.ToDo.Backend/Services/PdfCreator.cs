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
        public async Task<bool> CreateDailySheet(List<ToDoListEntry> todos, List<string> priorities, string forTomorrow, string note)
        {
            try
            {
                var filePath = @"C:\tmp\daily.pdf";
                var html = GetHtmlCode(todos, priorities, forTomorrow, note);

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

        private string GetHtmlCode(List<ToDoListEntry> todos, List<string> priorities, string forTomorrow, string note)
        {
            var htmlCode = string.Empty;

            htmlCode = Properties.Resources.Daily;
            htmlCode = htmlCode.Replace("{plan-date}", DateTime.Now.Date.ToShortDateString());

            // ToDo's
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

            // Appointments
            for (int i = 0; i < 14; i++)
            {
                // ToDo: change code to display appointments
                htmlCode = htmlCode.Replace("{appointment-" + (i + 1) + "}", string.Empty);
            }

            // Priorities
            for (int i = 0; i < 4; i++)
            {              
                if (i < priorities.Count)
                {
                    htmlCode = htmlCode.Replace("{priority-" + (i + 1) + "}", priorities[i]);
                }
                else
                {
                    htmlCode = htmlCode.Replace("{priority-" + (i + 1) + "}", string.Empty);
                }
            }

            htmlCode = htmlCode.Replace("{for-tomorrow}", forTomorrow);
            htmlCode = htmlCode.Replace("{note}", note);

            return htmlCode;
        }
    }
}
