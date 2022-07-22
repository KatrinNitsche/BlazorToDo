using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IBudgetService budgetService { get; set; }

        [Inject]
        public IToDoService toDoService { get; set; }

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

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
            var userId = await GetCurrentUserId();
            Tasks = (await toDoService.GetAll(userId));
            Appointments = (await appointmentService.GetAll(userId));

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
            var userId = await GetCurrentUserId();
            var taskList = await toDoService.GetAll(userId, PdfSettings.Date, PdfSettings.Date);

            if (PdfSettings.IncludeAppointments)
            {
                var AppointmentList = await appointmentService.GetAll(userId, PdfSettings.Date, PdfSettings.Date);
            }
            else
            {
                Appointments = new List<Appointment>();
            }

            var priorities = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
            if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
            if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
            if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);
                      
            var BudgetEntries = (await budgetService.GetAll(userId));           
            var fromDate = new DateTime(PdfSettings.Date.Year, PdfSettings.Date.Month, 1);
            var todate = fromDate.AddMonths(1).AddDays(-1);
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date <= todate);

            var html = pdfCreator.GetHtmlCodeForDayPlan(taskList, Appointments.ToList(), priorities, PdfSettings.ForTomorrow, PdfSettings.Note, PdfSettings.IncludeFincance, BudgetEntries.ToList());
            var pdf = Pdf
               .From(html)
               .OfSize(PaperSize.A4)
               .WithTitle($"Day Plan {PdfSettings.Date.ToShortDateString()}")
               .WithoutOutline()
               .WithMargins(1.25.Centimeters())
               .Portrait()
               .Comressed()
               .Content();

            await DownloadFileFromStream(pdf, $"Day Plan {PdfSettings.Date.ToShortDateString()}.pdf");
        }

        private async void WeekPlan()
        {
            var date = WeekVal; // "2022-W27"
            int.TryParse(WeekVal.Substring(0, 4), out int year);
            int.TryParse(WeekVal.Substring(6, 2).Replace("0", "").Trim(), out int monthNumber);
            var firstDayOfWeek = Tools.FirstDateOfWeekISO8601(year, monthNumber);
            var userId = await GetCurrentUserId();
            var taskList = await toDoService.GetAll(userId, firstDayOfWeek, firstDayOfWeek.AddDays(6));

            if (PdfSettings.IncludeAppointments)
            {               
                var AppointmentList = (await appointmentService.GetAll(userId));
                Appointments = AppointmentList.Where(a => a.Date >= firstDayOfWeek && a.Date <= firstDayOfWeek.AddDays(7));
            }
            else
            {
                Appointments = new List<Appointment>();
            }

            var priorities = new List<string>();
            if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
            if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
            if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
            if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);
                       
            var BudgetEntries = (await budgetService.GetAll(userId));
            var fromDate = new DateTime(PdfSettings.Date.Year, PdfSettings.Date.Month, 1);
            var todate = fromDate.AddMonths(1).AddDays(-1);
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date <= todate);

            var html = pdfCreator.GetHtmlCodeForWeekPlan(taskList, Appointments.ToList(), priorities, firstDayOfWeek, PdfSettings.IncludeFincance, BudgetEntries.ToList());
            var pdf = Pdf
              .From(html)
              .OfSize(PaperSize.A4)
              .WithTitle($"Week Plan")
              .WithoutOutline()
              .WithMargins(1.25.Centimeters())
              .Portrait()
              .Comressed()
              .Content();

            await DownloadFileFromStream(pdf, $"Week Plan.pdf");
        }

        private async void MonthPlan()
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

            var userId = await GetCurrentUserId();
            var BudgetEntries = (await budgetService.GetAll(userId));
            var fromDate = new DateTime(PdfSettings.Date.Year, PdfSettings.Date.Month, 1);
            var todate = fromDate.AddMonths(1).AddDays(-1);
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date <= todate);

            var html = pdfCreator.GetHtmlCodeForMonthPlan(PdfSettings.Note, importantSteps, MonthVal, PdfSettings.IncludeFincance, BudgetEntries.ToList());
            var pdf = Pdf
                .From(html)
                .OfSize(PaperSize.A4)
                .WithTitle($"Month Plan {PdfSettings.Date.Month} {PdfSettings.Date.Year}")
                .WithoutOutline()
                .WithMargins(1.25.Centimeters())
                .Portrait()
                .Comressed()
                .Content();

            await DownloadFileFromStream(pdf, $"Month Plan {PdfSettings.Date.Month} {PdfSettings.Date.Year}.pdf");
        }

        private async void YearPlan()
        {
            var firstDayofYear = new DateTime(int.Parse(PdfSettings.Year), 1, 1);          
            var html = pdfCreator.GetHtmlCodeForYearPlan(firstDayofYear);
            var pdf = Pdf
                .From(html)
                .OfSize(PaperSize.A4)
                .WithTitle($"Year Plan {PdfSettings.Year}")
                .WithoutOutline()
                .WithMargins(1.25.Centimeters())
                .Portrait()
                .Comressed()
                .Content();

            await DownloadFileFromStream(pdf, $"Year Plan {PdfSettings.Year}.pdf");
        }

        public async Task<Guid> GetCurrentUserId()
        {
            var user = (await authenticationStateTask).User;
            if (user.Identity.IsAuthenticated)
            {
                var currentUser = await userManager.GetUserAsync(user);
                var currentUserId = currentUser.Id;

                return Guid.Parse(currentUserId);
            }

            return Guid.Empty;
        }

        private Stream GetFileStream(byte[] file)
        {
            var fileStream = new MemoryStream(file);
            return fileStream;
        }

        private async Task DownloadFileFromStream(byte[] file, string fileName)
        {
            var fileStream = GetFileStream(file);
            using var streamRef = new DotNetStreamReference(stream: fileStream);
            await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
        }
    }
}
