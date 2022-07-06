using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class ToDoList
    {
        public IEnumerable<ToDoListEntry> Tasks { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";
        public bool DisplayOnlyTodaysTasks { get; set; }
        public bool DisplayDoneTasks { get; set; }

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        protected AddToDoEntryDialog AddToDoEntryDialog { get; set; }
        protected EditToDoEntryDialog EditToDoEntryDialog { get; set; }
        protected ExportToDosDialog ExportToDosDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Tasks = (await service.GetAll());
            Tasks = Tasks.Where(t => t.Done == false);
        }

        protected void ShowExportDialog()
        {
            ExportToDosDialog.Show();
        }

        protected void AddToDoEntry()
        {
            AddToDoEntryDialog.Show();
        }

        protected void EditToDoEntry(int id)
        {
            EditToDoEntryDialog.Show(id);
        }

        public async void AddToDoEntryDialog_OnDialogClose()
        {
            Tasks = (await service.GetAll());
            StateHasChanged();
        }

        public async void DeleteToDoEntry(int id)
        {
            var result = await service.Remove(id);

            if (result)
            {
                toastService.ShowSuccess("Entry was deleted.");
            }
            else
            {
                toastService.ShowError("Error while trying to delete the entry.");
            }
        }

        public void ToggleToDoDone(int id, object checkedValue)
        {
            service.ToggleDone(id);
        }

        public async void FilterList(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                Tasks = (await service.GetAll());
            }
            else
            {
                Tasks = Tasks.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            StateHasChanged();                
        }

        public async void SortByColumn(string columnName)
        {
            switch(columnName)
            {
                case "Date":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Date) : Tasks.OrderByDescending(t => t.Date);
                    break;
                case "Priority":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Priority) : Tasks.OrderByDescending(t => t.Priority);
                    break;
                case "Title":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Title) : Tasks.OrderByDescending(t => t.Title);
                    break;
                case "Done":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Done) : Tasks.OrderByDescending(t => t.Done);
                    break;
                case "Category":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Category?.Name) : Tasks.OrderByDescending(t => t.Category?.Name);
                    break;
            }

            SortingColumn = columnName;
            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
            StateHasChanged();
        }

        public async void ShowTasksFromToday()
        {
            DisplayOnlyTodaysTasks = !DisplayOnlyTodaysTasks;

            if (DisplayOnlyTodaysTasks)
            {
                Tasks = Tasks.Where(t => t.Date.Date == System.DateTime.Now.Date);
            }
            else
            {
                Tasks = (await service.GetAll());
            }

            if (DisplayDoneTasks)
            {
                Tasks = Tasks.Where(t => t.Done == true);
            }
            else
            {
                Tasks = Tasks.Where(t => t.Done == false);
            }

            StateHasChanged();
        }

        public async void ShowTasksDone()
        {
            DisplayDoneTasks = !DisplayDoneTasks;
            Tasks = (await service.GetAll());

            if (DisplayDoneTasks)
            {
                Tasks = Tasks.Where(t => t.Done == true);
            }
            else
            {
                Tasks = Tasks.Where(t => t.Done == false);
            }
        }
    }
}
