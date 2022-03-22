using Blazored.Toast.Services;
using CollaborateSoftware.ToDo.Backend.Data;
using CollaborateSoftware.ToDo.Backend.Services;
using CollaborateSoftware.ToDo.Components;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.ToDo.Pages
{
    public partial class ToDoList
    {
        public IEnumerable<ToDoListEntry> Tasks { get; set; }
        
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
    }
}
