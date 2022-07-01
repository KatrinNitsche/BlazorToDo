using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class EditToDoEntryDialog
    {
        public ToDoListEntry ToDoListEntry { get; set; } = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false, RepetitionType = RepetitionType.None };

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }

        public async void Show(int id)
        {
            ToDoListEntry = await service.GetById(id);
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
            ToDoListEntry = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false, RepetitionType = RepetitionType.None };
        }

        protected async Task HandleValidSubmit()
        {
            var result = await service.Update(ToDoListEntry);
            if (result != null)
            {
                ShowDialog = false;
                toastService.ShowSuccess("Entry was saved successfully");
                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to save entry.");
            }
        }
    }
}
