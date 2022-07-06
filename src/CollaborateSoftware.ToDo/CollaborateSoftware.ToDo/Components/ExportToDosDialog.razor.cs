using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Data;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class ExportToDosDialog
    {
        public ToDoExportSettings ExportSettings { get; set; } = new ToDoExportSettings { };

        public IEnumerable<Category> CategoryList { get; set; }

        public bool ShowDialog { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        [Inject]
        public IToDoService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }


        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            ExportSettings = new ToDoExportSettings();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            var result = service.Export(ExportSettings);
            if (result != null)
            {
                ShowDialog = false;
                toastService.ShowSuccess("To Dos where exported");
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to export.");
            }
        }
    }
}
