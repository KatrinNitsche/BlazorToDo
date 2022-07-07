using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class PdfCreationDialog
    {
        public PdfSettings PdfSettings { get; set; } = new PdfSettings() { Type = "Day" };

        public IEnumerable<ToDoListEntry> Tasks { get; set; }

        public bool ShowDialog { get; set; }

        [Inject]
        public IPdfCreator pdfCreator { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public IToDoService toDoService { get; set; }

        Dictionary<string, object> typeInput = new Dictionary<string, object> { { "type", "week" } };

        private string WeekVal { get; set; }

        public async void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            PdfSettings = new PdfSettings() { Type = "Day" };
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Tasks = (await toDoService.GetAll());

            if (PdfSettings.Type == "Day")
            {
                DayPlan();
            }
            else if (PdfSettings.Type == "Week")
            {
                WeekPlan();
            }
        }

        private void DayPlan()
        {
            Tasks = Tasks.Where(t => t.Date == PdfSettings.Date && t.Done == false);

            var priorities = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
            if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
            if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
            if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);

            var result = pdfCreator.CreateDailySheet(Tasks.ToList(), priorities, PdfSettings.ForTomorrow, PdfSettings.Note);
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
    }
}
