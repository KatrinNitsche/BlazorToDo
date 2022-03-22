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

        protected AddToDoEntryDialog AddToDoEntryDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Tasks = (await service.GetAll());
        }

        protected void AddToDoEntry()
        {
            AddToDoEntryDialog.Show();
        }

        public async void AddToDoEntryDialog_OnDialogClose()
        {
            Tasks = (await service.GetAll());
            StateHasChanged();
        }

        public void ToggleToDoDone(int id, object checkedValue)
        {
            service.ToggleDone(id);
        }
    }
}
