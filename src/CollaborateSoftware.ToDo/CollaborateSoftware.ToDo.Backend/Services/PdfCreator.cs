using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class PdfCreator : IPdfCreator
    {
        private int currentRecoursiveLevel = 0;

        public async Task<bool> CreateNotesPdf(List<NotesEntry> notesEntries, string notesTitle)
        {
            try
            {
                var filePath = @"C:\tmp\notes.pdf";
                var html = GetHtmlCodeForNotes(notesEntries, notesTitle);

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

        public async Task<bool> CreateDailySheet(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, string forTomorrow, string note)
        {
            try
            {
                var filePath = @"C:\tmp\daily.pdf";
                var html = GetHtmlCodeForDayPlan(todos, appointments, priorities, forTomorrow, note);

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

        public async Task<bool> CreateWeekPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, DateTime firstDayOfWeek)
        {
            try
            {
                var filePath = @"C:\tmp\weekly.pdf";
                var html = GetHtmlCodeForWeekPlan(todos, appointments, priorities, firstDayOfWeek);

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

        public async Task<bool> CreateMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth)
        {
            try
            {
                var filePath = @"C:\tmp\monthly.pdf";
                var html = GetHtmlCodeForMonthPlan(focusText, actionSteps, firstDayOfMonth);

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

        public async Task<bool> CreateYearPlan(DateTime firstDayofYear)
        {
            try
            {
                var filePath = @"C:\tmp\yearly.pdf";
                var html = GetHtmlCodeForYearPlan(firstDayofYear);

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

        private string GetHtmlCodeForNotes(List<NotesEntry> notesEntries, string notesTitle)
        {
            string htmlCode = string.Empty;
            htmlCode = Properties.Resources.Notes;

            var notesDiv = string.Empty;
            foreach (var note in notesEntries.Where(n => n.ParentNoteId == null))
            {
                notesDiv += "<div>" + Environment.NewLine;
                notesDiv += "<div class='note'>" + Environment.NewLine;
                notesDiv += $"<h2>{note.Title}</h2>" + Environment.NewLine;
                notesDiv += $"<p>{note.Text} </p>" + Environment.NewLine;
                notesDiv += "</div>" + Environment.NewLine;

                currentRecoursiveLevel = 1;
                notesDiv += GetChildNoteHtml(notesEntries, note.Id);
                notesDiv += "</div" + Environment.NewLine;
            }

            htmlCode = htmlCode.Replace("{notes}", notesDiv);
            htmlCode = htmlCode.Replace("{notes-title}", notesTitle);
            return htmlCode;
        }

        private string GetChildNoteHtml(List<NotesEntry> notesEntries, int parentId)
        {
            var notesDiv = string.Empty;
            foreach (var note in notesEntries.Where(n => n.ParentNoteId == parentId))
            {
                notesDiv += "<div style='margin-left: " + (currentRecoursiveLevel * 5) + "px'>" + Environment.NewLine;
                notesDiv += "<div class='note-child'>" + Environment.NewLine;
                notesDiv += $"<h2>{note.Title}</h2>" + Environment.NewLine;
                notesDiv += $"<p>{note.Text} </p>" + Environment.NewLine;
                notesDiv += "</div>" + Environment.NewLine;
              
                if (notesEntries.Any(n => n.ParentNoteId == note.Id))
                {
                    currentRecoursiveLevel++;
                    notesDiv += GetChildNoteHtml(notesEntries, note.Id);
                }
                
                notesDiv += "</div" + Environment.NewLine;
            }

            return notesDiv;
        }

        private string GetHtmlCodeForMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth)
        {
            var htmlCode = string.Empty;
            htmlCode = Properties.Resources.Monthly;

            htmlCode = htmlCode.Replace("{Month}", $"{firstDayOfMonth.ToMonthName()} {firstDayOfMonth.Year}");
            htmlCode = htmlCode.Replace("{notes}", focusText);
            htmlCode = ReplaceActionSteps(actionSteps, htmlCode, 10);

            var firstDayIndex = GetFirstDayIndex(firstDayOfMonth);
            var numberOfDays = firstDayOfMonth.AddMonths(1).AddDays(-1).Day;
            for (int i = 0; i < 42; i++)
            {
                if (i < firstDayIndex || i - firstDayIndex >= numberOfDays)
                {
                    htmlCode = htmlCode.Replace("{day-" + (i + 1) + "}", string.Empty);
                }
                else
                {
                    htmlCode = htmlCode.Replace("{day-" + (i + 1) + "}", (i + 1 - firstDayIndex).ToString());
                }
            }

            return htmlCode;
        }

        private int GetFirstDayIndex(DateTime firstDayOfMonth)
        {
            var weekDay = firstDayOfMonth.Date.DayOfWeek;
            return (int)weekDay;
        }

        private string GetHtmlCodeForWeekPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, DateTime firstDayOfWeek)
        {
            var htmlCode = string.Empty;
            htmlCode = Properties.Resources.Weekly;
            htmlCode = htmlCode.Replace("{dates}", GetAppointmentForDay(appointments));

            for (int i = 0; i < 7; i++)
            {
                htmlCode = htmlCode.Replace("{day-" + (i + 1) + "}", firstDayOfWeek.AddDays(i).ToString("dddd, MMMM dd yyyy"));
            }

            htmlCode = ReplaceToDos(todos, htmlCode, 18);
            htmlCode = ReplacePriorities(priorities, htmlCode);

            return htmlCode;
        }

        private string GetHtmlCodeForDayPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, string forTomorrow, string note)
        {
            var htmlCode = string.Empty;

            htmlCode = Properties.Resources.Daily;
            htmlCode = htmlCode.Replace("{plan-date}", DateTime.Now.Date.ToShortDateString());

            htmlCode = ReplaceToDos(todos, htmlCode, 9);

            for (int i = 0; i < 14; i++)
            {
                htmlCode = htmlCode.Replace("{appointment-" + (i + 1) + "}", GetAppointmentForHour(i, appointments));
            }

            htmlCode = ReplacePriorities(priorities, htmlCode);
            htmlCode = htmlCode.Replace("{for-tomorrow}", forTomorrow);
            htmlCode = htmlCode.Replace("{note}", note);

            return htmlCode;
        }

        private string GetHtmlCodeForYearPlan(DateTime firstDayOfYear)
        {
            var htmlCode = string.Empty;
            htmlCode = Properties.Resources.Year;

            htmlCode = htmlCode.Replace("{year}", $"{firstDayOfYear.Year}");

            for (int i = 0; i < 12; i++)
            {
                var firstDayIndex = GetFirstDayIndex(firstDayOfYear.AddMonths(i));
                var numberOfDays = firstDayOfYear.AddMonths(i).AddDays(-1).Day;
                for (int y = 0; y < 42; y++)
                {
                    if (y < firstDayIndex || y >= numberOfDays)
                    {
                        htmlCode = htmlCode.Replace("{day-" + (i + 1) + "-" + (y + 1) + "}", string.Empty);
                    }
                    else
                    {
                        htmlCode = htmlCode.Replace("{day-" + (i + 1) + "-" + (y + 1) + "}", (y + 1 - firstDayIndex).ToString());
                    }
                }
            }

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

        private static string ReplaceActionSteps(List<string> steps, string htmlCode, int numberOfEntries)
        {
            for (int i = 0; i < numberOfEntries; i++)
            {
                if (i < steps.Count)
                {
                    htmlCode = htmlCode.Replace("{todo-text-" + (i + 1) + "}", steps[i]);
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

        private static string GetAppointmentForHour(int index, List<Appointment> appointments)
        {
            var result = string.Empty;

            var appointmentsInSlot = new List<Appointment>();
            int fromTime = index + 6;
            int toTime = index + 7;

            appointmentsInSlot = appointments.Where(a => a.Date.Hour >= fromTime && a.Date.Hour < toTime).ToList();

            foreach (var entry in appointmentsInSlot)
            {
                var hour = entry.Date.Hour < 10 ? $"0{entry.Date.Hour}" : entry.Date.Hour.ToString();
                var minutes = entry.Date.Minute < 10 ? $"0{entry.Date.Minute}" : entry.Date.Minute.ToString();
                var time = $"{hour}:{minutes}";
                result += $"{time} {entry.Title}, ";
            }

            if (result.EndsWith(", "))
            {
                result = result.Substring(0, result.Length - 2);
            }

            return result;
        }

        private string GetAppointmentForDay(List<Appointment> appointments)
        {
            var result = string.Empty;
            foreach (var entry in appointments)
            {
                result += $"{entry.Date} {entry.Title}" + System.Environment.NewLine;
            }

            return result;
        }
    }
}
