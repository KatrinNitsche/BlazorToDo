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
                var html = GetHtmlCodeForDayPlan(todos, priorities, forTomorrow, note);

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
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateWeekPlan(List<ToDoListEntry> todos, List<string> priorities, DateTime firstDayOfWeek)
        {
            try
            {
                var filePath = @"C:\tmp\weekly.pdf";
                var html = GetHtmlCodeForWeekPlan(todos, priorities, firstDayOfWeek);

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
            catch (Exception ex)
            {
                return false;
            }
        }

        private string GetHtmlCodeForWeekPlan(List<ToDoListEntry> todos, List<string> priorities, DateTime firstDayOfWeek)
        {
            var htmlCode = string.Empty;
            htmlCode = Properties.Resources.Weekly;

            // ToDo: replace dates
            htmlCode = htmlCode.Replace("{dates}", string.Empty);

            // Day names
            for (int i = 0; i < 7; i++)
            {
                htmlCode = htmlCode.Replace("{day-" + (i+1) + "}", firstDayOfWeek.AddDays(i).ToString("dddd, MMMM dd yyyy"));
            }

            // ToDo's 
            htmlCode = ReplaceToDos(todos, htmlCode, 18);

            // Priorities
            htmlCode = ReplacePriorities(priorities, htmlCode);

            return htmlCode;
        }

        private string GetHtmlCodeForDayPlan(List<ToDoListEntry> todos, List<string> priorities, string forTomorrow, string note)
        {
            var htmlCode = string.Empty;

            htmlCode = Properties.Resources.Daily;
            htmlCode = htmlCode.Replace("{plan-date}", DateTime.Now.Date.ToShortDateString());

            // ToDo's
            htmlCode = ReplaceToDos(todos, htmlCode, 9);

            // Appointments
            for (int i = 0; i < 14; i++)
            {
                // ToDo: change code to display appointments
                htmlCode = htmlCode.Replace("{appointment-" + (i + 1) + "}", string.Empty);
            }

            // Priorities
            htmlCode = ReplacePriorities(priorities, htmlCode);

            htmlCode = htmlCode.Replace("{for-tomorrow}", forTomorrow);
            htmlCode = htmlCode.Replace("{note}", note);

            return htmlCode;
        }

        private static string ReplaceToDos(List<ToDoListEntry> todos, string htmlCode, int numberOfEntries)
        {
            for (int i = 0; i < numberOfEntries; i++)
            {
                if (i < todos.Count)
                {
                    var title = string.Empty;
                    if (todos[i].Priority == Priority.High) title += "! ";
                    title += todos[i].Title;

                    htmlCode = htmlCode.Replace("{todo-text-" + (i + 1) + "}", title);
                }
                else
                {
                    htmlCode = htmlCode.Replace("{todo-text-" + (i + 1) + "}", string.Empty);
                }
            }

            return htmlCode;
        }

        private static string ReplacePriorities(List<string> priorities, string htmlCode)
        {
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

            return htmlCode;
        }
    }
}
