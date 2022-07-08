using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class AddToDoEntryDialog
    {
        public ToDoListEntry ToDoListEntry { get; set; } = new ToDoListEntry { Title = string.Empty, Date = DateTime.Now, Done = false, Priority = Priority.Middle, RepetitionType = RepetitionType.None };
        public IEnumerable<Category> CategoryList { get; set; }

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }
       
        [Inject]
        public ICategoryService categoryService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }
        
        public string CurrentCategoryId { get; set; }

        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
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
            ToDoListEntry = new ToDoListEntry { Title = string.Empty, Date = DateTime.Now, Done = false, Priority = Priority.Middle, RepetitionType = RepetitionType.None };
        }

        protected async Task HandleValidSubmit()
        {
            ToDoListEntry.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
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
