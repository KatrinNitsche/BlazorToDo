using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
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


        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }


        protected AddCategoryDialog AddCategoryDialog { get; set; }
        protected EditCategoryDialog EditCategoryDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var userId = await GetCurrentUserId();
            CategoryList = (await service.GetAll(userId));
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
            var userId = await GetCurrentUserId();
            CategoryList = (await service.GetAll(userId));
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

            var userId = await GetCurrentUserId();
            CategoryList = (await service.GetAll(userId));
        }

        public async void AddCategoryDialog_OnDialogClose()
        {
            var userId = await GetCurrentUserId();
            CategoryList = (await service.GetAll(userId));
            StateHasChanged();
        }
                
        public async Task<Guid> GetCurrentUserId()
        {
            var user = (await authenticationStateTask).User;
            if (user.Identity.IsAuthenticated)
            {
                var currentUser = await userManager.GetUserAsync(user);
                var currentUserId = currentUser.Id;

                return Guid.Parse(currentUserId);
            }

            return Guid.Empty;
        }
    }
}
