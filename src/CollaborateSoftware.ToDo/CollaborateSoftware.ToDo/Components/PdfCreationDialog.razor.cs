using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class PdfCreationDialog
    {
        public PdfSettings PdfSettings { get; set; } = new PdfSettings() { Type = "Day", Date = DateTime.Now, Year = DateTime.Now.Year.ToString(), IncludeAppointments = false };

        public IEnumerable<ToDoListEntry> Tasks { get; set; }
        public IEnumerable<Appointment> Appointments { get; set; }

        public bool ShowDialog { get; set; }

        [Inject]
        public IPdfCreator pdfCreator { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public IAppointmentService appointmentService { get; set; }

        [Inject]
        public IToDoService toDoService { get; set; }

        Dictionary<string, object> typeInput = new Dictionary<string, object> { { "type", "week" } };

        private string WeekVal { get; set; }

        private DateTime MonthVal { get; set; }

        public async void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            PdfSettings = new PdfSettings() { Type = "Day", Date = DateTime.Now, Year = DateTime.Now.Year.ToString(), IncludeAppointments = false };
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Tasks = (await toDoService.GetAll());
            Appointments = (await appointmentService.GetAll());

            if (PdfSettings.Type == "Day")
            {
                DayPlan();
            }
            else if (PdfSettings.Type == "Week")
            {
                WeekPlan();
            }
            else if (PdfSettings.Type == "Month")
            {
                MonthPlan();
            }
            else if (PdfSettings.Type == "Year")
            {
                YearPlan();
            }
        }

        private async void DayPlan()
        {
            Tasks = Tasks.Where(t => t.Date == PdfSettings.Date && t.Done == false);
            var AppointmentList = (await appointmentService.GetAll());
            Appointments = AppointmentList.Where(a => a.Date.Year == PdfSettings.Date.Year && 
                                                      a.Date.Month == PdfSettings.Date.Month &&
                                                      a.Date.Day == PdfSettings.Date.Day).OrderBy(a => a.Date).ToList();

            var priorities = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
            if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
            if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
            if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);

            var result = pdfCreator.CreateDailySheet(Tasks.ToList(), Appointments.ToList(), priorities, PdfSettings.ForTomorrow, PdfSettings.Note);
            if (result != null)
            {
                toastService.ShowSuccess("Pdf document was created");
                ShowDialog = false;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to create pdf document.");
            }
        }

        private void WeekPlan()
        {
            var date = WeekVal; // "2022-W27"
            int.TryParse(WeekVal.Substring(0, 4), out int year);
            int.TryParse(WeekVal.Substring(6, 2).Replace("0","").Trim(), out int monthNumber);
            var firstDayOfWeek = Tools.FirstDateOfWeekISO8601(year, monthNumber);

            Tasks = Tasks.Where(t => t.Date >= firstDayOfWeek && t.Date <= firstDayOfWeek.AddDays(7) && t.Done == false).OrderBy(t => t.Date).ThenBy(t => t.Title);

            var priorities = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
            if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
            if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
            if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);

            var result = pdfCreator.CreateWeekPlan(Tasks.ToList(), priorities, firstDayOfWeek);
            if (result != null)
            {
                toastService.ShowSuccess("Pdf document was created");
                ShowDialog = false;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to create pdf document.");
            }
        }

        private void MonthPlan()
        {
            var importantSteps = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep1)) importantSteps.Add(PdfSettings.ActionStep1);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep2)) importantSteps.Add(PdfSettings.ActionStep2);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep3)) importantSteps.Add(PdfSettings.ActionStep3);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep4)) importantSteps.Add(PdfSettings.ActionStep4);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep5)) importantSteps.Add(PdfSettings.ActionStep5);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep6)) importantSteps.Add(PdfSettings.ActionStep6);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep7)) importantSteps.Add(PdfSettings.ActionStep7);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep8)) importantSteps.Add(PdfSettings.ActionStep8);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep9)) importantSteps.Add(PdfSettings.ActionStep8);
            if (!string.IsNullOrEmpty(PdfSettings.ActionStep10)) importantSteps.Add(PdfSettings.ActionStep10);

            var result = pdfCreator.CreateMonthPlan(PdfSettings.Note, importantSteps, MonthVal);
            if (result != null)
            {
                toastService.ShowSuccess("Pdf document was created");
                ShowDialog = false;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to create pdf document.");
            }
        }

        private void YearPlan()
        {
            var firstDayofYear = new DateTime(int.Parse(PdfSettings.Year), 1, 1);
            var result = pdfCreator.CreateYearPlan(firstDayofYear);
            if (result != null)
            {
                toastService.ShowSuccess("Pdf document was created");
                ShowDialog = false;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to create pdf document.");
            }

        }
    }
}
