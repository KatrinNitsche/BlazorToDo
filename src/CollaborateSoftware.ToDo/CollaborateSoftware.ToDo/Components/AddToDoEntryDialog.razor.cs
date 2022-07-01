using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class AddToDoEntryDialog
    {
        public ToDoListEntry ToDoListEntry { get; set; } = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false, Priority = Priority.Middle, RepetitionType = RepetitionType.None };

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

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
            ToDoListEntry = new ToDoListEntry { Title = "New Task", Date = DateTime.Now, Done = false, Priority = Priority.Middle, RepetitionType = RepetitionType.None };
        }

        protected async Task HandleValidSubmit()
        {
            var result = await service.Add(ToDoListEntry);
            if (result != null)
            {
                ShowDialog = false;
                toastService.ShowSuccess("Entry was added successfully");

                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to add entry.");
            }
        }
    }
}
