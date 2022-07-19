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
    public partial class Budget
    {
        public IEnumerable<BudgetEntry> BudgetEntries { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";
        public bool DisplayOnlyThisMonthsEntries { get; set; } = true;

        [Inject]
        public IBudgetService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        public AddBudgetEntryDialog AddBudgetEntryDialog { get; set; }
        public EditBudgetEntryDialog EditBudgetEntryDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var userId = await GetCurrentUserId();
            BudgetEntries = (await service.GetAll(userId));
        }

        public async void FilterList(string searchTerm)
        {
            SearchTerm = searchTerm;
            var userId = await GetCurrentUserId();
            BudgetEntries = (await service.GetAll(userId));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                BudgetEntries = BudgetEntries.Where(t => t.Description.ToLower().Contains(searchTerm.ToLower()));
            }

            if (DisplayOnlyThisMonthsEntries)
            {
                // ToDo: get this months date range
                BudgetEntries = BudgetEntries.Where(t => t.BudgetDate.Date == System.DateTime.Now.Date);
            }

            StateHasChanged();
        }

        public async void SortByColumn(string columnName)
        {
            SortingColumn = columnName;
            switch (SortingColumn)
            {
                case "Date":
                    BudgetEntries = SortingDirection == "Asc" ? BudgetEntries.OrderBy(t => t.BudgetDate) : BudgetEntries.OrderByDescending(t => t.BudgetDate);
                    break;
                case "Amount":
                    BudgetEntries = SortingDirection == "Asc" ? BudgetEntries.OrderBy(t => t.Amount) : BudgetEntries.OrderByDescending(t => t.Amount);
                    break;
                case "Description":
                    BudgetEntries = SortingDirection == "Asc" ? BudgetEntries.OrderBy(t => t.Description) : BudgetEntries.OrderByDescending(t => t.Description);
                    break;              
                case "Category":
                    BudgetEntries = SortingDirection == "Asc" ? BudgetEntries.OrderBy(t => t.Category?.Name) : BudgetEntries.OrderByDescending(t => t.Category?.Name);
                    break;
            }

            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
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

        protected void AddEntry()
        {
            AddBudgetEntryDialog.Show();
        }

        protected void EditEntry(int id)
        {
            EditBudgetEntryDialog.Show(id);
        }

        public async void AddToDoEntryDialog_OnDialogClose()
        {
            FilterList(SearchTerm);
            SortByColumn(SortingColumn);
            StateHasChanged();
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
    }
}
