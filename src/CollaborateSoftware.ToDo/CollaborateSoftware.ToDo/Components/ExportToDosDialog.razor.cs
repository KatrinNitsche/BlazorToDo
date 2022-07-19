using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
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

        #region user

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        #endregion


        public async void Show()
        {
            var userId = await GetCurrentUserId();
            CategoryList = await categoryService.GetAll(userId);
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
