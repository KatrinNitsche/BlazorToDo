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
    public partial class Categories
    {
        public IEnumerable<Category> CategoryList { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";

        [Inject]
        public ICategoryService service { get; set; }
        
        [Inject]
        public IToastService toastService { get; set; }

        protected AddCategoryDialog AddCategoryDialog { get; set; }
        protected EditCategoryDialog EditCategoryDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            CategoryList = (await service.GetAll());
        }

        protected void AddCategory()
        {
            AddCategoryDialog.Show();
        }

        protected void EditCategory(int id)
        {
            EditCategoryDialog.Show(id);
        }

        protected async void SortByColumn(string columnName)
        {
            CategoryList = (await service.GetAll());
        }

        public async void DeleteCategory(int id)
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

            CategoryList = (await service.GetAll());
        }

        public async void AddCategoryDialog_OnDialogClose()
        {
            CategoryList = (await service.GetAll());
            StateHasChanged();
        }
    }
}
