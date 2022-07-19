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
    public partial class Habits
    {
        public IEnumerable<Habit> HabitList { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";

        [Inject]
        public IHabitService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        protected AddHabitDialog AddHabitDialog { get; set; }
        protected EditHabitDialog EditHabitDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var userId = await GetCurrentUserId();
            HabitList = (await service.GetAll(userId));          
        }

        protected void AddEntry()
        {
            AddHabitDialog.Show();
        }

        public async void FilterList(string searchTerm)
        {
            var userId = await GetCurrentUserId();
            SearchTerm = searchTerm;
            HabitList = (await service.GetAll(userId));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                HabitList = HabitList.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()));
            }                     

            StateHasChanged();
        }

        public async void SortByColumn(string columnName)
        {
            SortingColumn = columnName;
            switch (SortingColumn)
            {               
                case "Title":
                    HabitList = SortingDirection == "Asc" ? HabitList.OrderBy(t => t.Title) : HabitList.OrderByDescending(t => t.Title);
                    break;
                case "Category":
                    HabitList = SortingDirection == "Asc" ? HabitList.OrderBy(t => t.Category?.Name) : HabitList.OrderByDescending(t => t.Category?.Name);
                    break;
            }

            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
            StateHasChanged();
        }

        public async void AddEntryDialog_OnDialogClose()
        {
            FilterList(SearchTerm);
            SortByColumn(SortingColumn);
            StateHasChanged();
        }

        protected void EditEntry(int id)
        {
            EditHabitDialog.Show(id);
        }

        public async void DeleteEntry(int id)
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
