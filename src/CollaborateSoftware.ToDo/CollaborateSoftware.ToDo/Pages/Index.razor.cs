using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class Index
    {
        #region Habits

        public IEnumerable<Habit> HabitList { get; set; }

        [Inject]
        public IHabitService habitService { get; set; }

        public DateTime FirstDayOfCurrentWeek { get; set; }

        public Dictionary<int, List<string>> HabitStates { get; set; }

        #endregion

        #region ToDo's

        public IEnumerable<ToDoListEntry> Tasks { get; set; }

        [Inject]
        public IToDoService todoService { get; set; }

        #endregion

        #region appointments

        public IEnumerable<Appointment> AppointmentList { get; set; }

        [Inject]
        public IAppointmentService service { get; set; }

        #endregion

        #region Budget

        public IEnumerable<BudgetEntry> BudgetEntries { get; set; }

        [Inject]
        public IBudgetService budgetService { get; set; }

        #endregion

        #region user

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        #endregion

        [Inject]
        public IPdfCreator pdfCreator { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await LoadHabits();
            var userId = await GetCurrentUserId();
            Tasks = (await todoService.GetAll(userId));
            Tasks = Tasks.Where(t => t.Date.Date == DateTime.Now.Date);

            AppointmentList = (await service.GetAll(userId));
            AppointmentList = AppointmentList.Where(a => a.Date.Date == DateTime.Now.Date);

            var fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var todate = fromDate.AddMonths(1).AddDays(-1);

            BudgetEntries = (await budgetService.GetAll(userId));
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date <= todate);
        }

        public string Balance()
        {
            var balance = BudgetEntries.Sum(b => b.Amount);
            return string.Format("{0:C}", balance);
        }

        private async Task LoadHabits()
        {
            var userId = await GetCurrentUserId();
            HabitList = (await habitService.GetAll(userId));
            FirstDayOfCurrentWeek = Tools.MondayBefore(DateTime.Now);

            HabitStates = new Dictionary<int, List<string>>();
            foreach (var habit in HabitList)
            {
                var habitStatesForWeek = new List<string>();
                for (int i = 0; i < 7; i++)
                {
                    var done = HabitList.Any(h => h.Id == habit.Id && h.DoneOnDay(FirstDayOfCurrentWeek.AddDays(i)));

                    if (done)
                    {
                        habitStatesForWeek.Add("success");
                    }
                    else
                    {
                        if (FirstDayOfCurrentWeek.AddDays(i) >= DateTime.Now.Date)
                        {
                            habitStatesForWeek.Add("transparent");
                        }
                        else
                        {
                            habitStatesForWeek.Add("danger");
                        }
                    }
                }

                HabitStates.Add(habit.Id, habitStatesForWeek);
            }
        }

        protected string ColourForDay(int habitId, int index)
        {
            return HabitStates[habitId][index];
        }

        public async void ToggleHabitDone(int id)
        {
            var result = habitService.DoneToday(id);
            if (result)
            {
                toastService.ShowSuccess("Habit was done");
            }
            else
            {
                toastService.ShowError("Error while trying to delete the entry.");
            }

            var userId = await GetCurrentUserId();
            HabitList = (await habitService.GetAll(userId));
            StateHasChanged();
        }

        public async void ToggleToDotDone(int id)
        {
            todoService.ToggleDone(id);
            var userId = await GetCurrentUserId();
            HabitList = (await habitService.GetAll(userId));
            StateHasChanged();
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

        #region PDF creation

        public async Task DayPLan()
        {
            var userId = await GetCurrentUserId();
            var taskList = await todoService.GetAll(userId);
            taskList = taskList.Where(t => t.Date.Date == DateTime.Now.Date);

            var AppointmentList = await service.GetAll(userId);
            AppointmentList = AppointmentList.Where(a => a.Date == DateTime.Now.Date);

            var priorities = new List<string>();

            var BudgetEntries = (await budgetService.GetAll(userId));
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date == DateTime.Now.Date);
                  
            var html = pdfCreator.GetHtmlCodeForDayPlan(taskList.ToList(), AppointmentList.ToList(), priorities, string.Empty, string.Empty, true, BudgetEntries.ToList());
            var fileName = $"Day Plan for {DateTime.Now.ToLongDateString()}";
            var pdf = Pdf
                .From(html)
                .OfSize(PaperSize.A4)
                .WithTitle(fileName)
                .WithoutOutline()
                .WithMargins(1.25.Centimeters())
                .Portrait()
                .Comressed()
                .Content();

            await DownloadFileFromStream(pdf, $"{fileName}.pdf");
        }

        public async Task WeekPLan()
        {         
            var firstDayOfWeek = Tools.MondayBefore(DateTime.Now.Date);
            var userId = await GetCurrentUserId();
            var taskList = await todoService.GetAll(userId, firstDayOfWeek, firstDayOfWeek.AddDays(6));

            var AppointmentList = (await service.GetAll(userId));
            var Appointments = AppointmentList.Where(a => a.Date >= firstDayOfWeek && a.Date <= firstDayOfWeek.AddDays(7));

            var priorities = new List<string>();          

            var BudgetEntries = (await budgetService.GetAll(userId));
            var fromDate = Tools.MondayBefore(DateTime.Now);
            var todate = fromDate.AddDays(7);
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date < todate);

            var html = pdfCreator.GetHtmlCodeForWeekPlan(taskList, Appointments.ToList(), priorities, firstDayOfWeek, true, BudgetEntries.ToList());
            var fileName = $"Week Plan";
            var pdf = Pdf
               .From(html)
               .OfSize(PaperSize.A4)
               .WithTitle(fileName)
               .WithoutOutline()
               .WithMargins(1.25.Centimeters())
               .Portrait()
               .Comressed()
               .Content();

            await DownloadFileFromStream(pdf, $"{fileName}.pdf");
        }

        public async Task MonthPlan()
        {
            var importantSteps = new List<string>();           

            var userId = await GetCurrentUserId();
            var BudgetEntries = (await budgetService.GetAll(userId));
            var fromDate = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, 1);
            var todate = fromDate.AddMonths(1).AddDays(-1);
            BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date >= fromDate && t.BudgetDate.Date <= todate);

            var html = pdfCreator.GetHtmlCodeForMonthPlan(string.Empty, importantSteps, fromDate, true, BudgetEntries.ToList());
            var fileName = $"Month Plan";
            var pdf = Pdf
                .From(html)
                .OfSize(PaperSize.A4)
                .WithTitle(fileName)
                .WithoutOutline()
                .WithMargins(1.25.Centimeters())
                .Portrait()
                .Comressed()
                .Content();

            await DownloadFileFromStream(pdf,$"{fileName}.pdf");
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

        #endregion 
    }
}
