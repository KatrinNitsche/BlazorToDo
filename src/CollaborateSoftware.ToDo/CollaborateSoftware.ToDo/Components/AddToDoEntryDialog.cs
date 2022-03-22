using CollaborateSoftware.ToDo.Backend.Data;
using CollaborateSoftware.ToDo.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace CollaborateSoftware.ToDo.Components
{
    public partial class AddToDoEntryDialog
    {
        public ToDoListEntry ToDoListEntry { get; set; } = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false };

        [Inject]
        public IToDoService service { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }
        public bool ShowDialog { get; set; }

        public void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            ToDoListEntry = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false };
        }

        protected async Task HandleValidSubmit()
        {
            await service.Add(ToDoListEntry);
            ShowDialog = false;

            await CloseEventCallback.InvokeAsync(true);
            StateHasChanged();
        }
    }
}
