using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
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
        public IToDoService service { get; set; }

        #endregion

        protected async override Task OnInitializedAsync()
        {
            await LoadHabits();
            Tasks = (await service.GetAll());
            Tasks = Tasks.Where(t => t.Date.Date == DateTime.Now.Date);
        }

        private async Task LoadHabits()
        {
            HabitList = (await habitService.GetAll());
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

        public async void ToggleToDoDone(int id)
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

            HabitList = (await habitService.GetAll());
            StateHasChanged();
        }
    }
}
