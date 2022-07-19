using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class EditBudgetEntryDialog
    {
        public BudgetEntry BudgetEntry { get; set; } = new BudgetEntry();
        public IEnumerable<Category> CategoryList { get; set; }

        [Inject]
        public IBudgetService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        #region user

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        #endregion

        public bool ShowDialog { get; set; }
        public string CurrentCategoryId { get; set; }

        public async void Show(int id)
        {
            var userId = await GetCurrentUserId();
            CategoryList = (await categoryService.GetAll(userId));
            BudgetEntry = await service.GetById(id);
            if (BudgetEntry.Category == null)
            {
                BudgetEntry.Category = CategoryList.FirstOrDefault(c => c.Name == "None");
            }

            CurrentCategoryId = BudgetEntry.Category.Id.ToString();
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
            BudgetEntry = new BudgetEntry();
        }

        protected async Task HandleValidSubmit()
        {
            BudgetEntry.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            BudgetEntry.Updated = DateTime.Now;
            var result = await service.Update(BudgetEntry);
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
