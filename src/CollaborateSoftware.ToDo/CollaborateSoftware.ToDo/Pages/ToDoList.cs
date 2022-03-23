using Blazored.Toast.Services;
using CollaborateSoftware.ToDo.Backend.Data;
using CollaborateSoftware.ToDo.Backend.Services;
using CollaborateSoftware.ToDo.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.ToDo.Pages
{
    public partial class ToDoList
    {
        public IEnumerable<ToDoListEntry> Tasks { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        protected AddToDoEntryDialog AddToDoEntryDialog { get; set; }
        protected EditToDoEntryDialog EditToDoEntryDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Tasks = (await service.GetAll());
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

        public async void SortByColum(string columnName)
        {
            switch(columnName)
            {
                case "Date":
                    Tasks = Tasks.OrderBy(t => t.Date);
                    break;
                case "Priority":
                    Tasks = Tasks.OrderBy(t => t.Priority);
                    break;
                case "Title":
                    Tasks = Tasks.OrderBy(t => t.Title);
                    break;
                case "Done":
                    Tasks = Tasks.OrderBy(t => t.Done);
                    break;
            }

            SortingColumn = columnName;
            StateHasChanged();
        }
    }
}
