using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class EditCategoryDialog
    {
        public Category Category { get; set; } = new Category { Name = "New Category" };

        [Inject]
        public ICategoryService service { get; set; }

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

        public async void Show(int id)
        {
            Category = await service.GetById(id);
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
            Category = new Category { Name = "New Category" };
        }

        protected async Task HandleValidSubmit()
        {
            Category.Updated = DateTime.Now;
            var result = await service.Update(Category);
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
