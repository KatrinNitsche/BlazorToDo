﻿using Blazored.Toast.Services;
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
    public partial class EditHabitDialog
    {
        public Habit Habit { get; set; } = new Habit();
        public IEnumerable<Category> CategoryList { get; set; }
        public bool ShowDialog { get; set; }

        [Inject]
        public IHabitService service { get; set; }

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

        public string CurrentCategoryId { get; set; }

        public async void Show()
        {
            var userId = await GetCurrentUserId();
            CategoryList = await categoryService.GetAll(userId);
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public async void Show(int id)
        {
            var userId = await GetCurrentUserId();
            CategoryList = (await categoryService.GetAll(userId));
            Habit = await service.GetById(id);
            if (Habit.Category == null)
            {
                Habit.Category = CategoryList.FirstOrDefault(c => c.Name == "None");
            }

            CurrentCategoryId = Habit.Category.Id.ToString();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            Habit = new Habit();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Habit.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            Habit.Updated = DateTime.Now;
            var result = await service.Update(Habit);
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
