using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
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
    }
}
