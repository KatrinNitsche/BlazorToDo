using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class ToDoList
    {
        public IEnumerable<ToDoListEntry> Tasks { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";
        public bool DisplayOnlyTodaysTasks { get; set; } = true;
        public bool DisplayDoneTasks { get; set; }

        [Inject]
        public IToDoService service { get; set; }
        
        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        protected AddToDoEntryDialog AddToDoEntryDialog { get; set; }
        protected EditToDoEntryDialog EditToDoEntryDialog { get; set; }
        protected ExportToDosDialog ExportToDosDialog { get; set; }
        protected PdfCreationDialog PdfCreationDialog { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var userId = await GetCurrentUserId();
            Tasks = (await service.GetAll(userId));
            Tasks = Tasks.Where(t => t.Done == false);
        }

        protected void ShowExportDialog()
        {
            ExportToDosDialog.Show();
        }

        public void PdfExport()
        {
            PdfCreationDialog.Show();
        }

        protected void AddToDoEntry()
        {
            AddToDoEntryDialog.Show();
        }

        protected void EditToDoEntry(int id)
        {
            EditToDoEntryDialog.Show(id);
        }

        public async void AddToDoEntryDialog_OnDialogClose()
        {
            FilterList(SearchTerm);
            SortByColumn(SortingColumn);
            StateHasChanged();
        }

        public async void DeleteToDoEntry(int id)
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

        public void ToggleToDoDone(int id, object checkedValue)
        {
            service.ToggleDone(id);
        }

        public async void FilterList(string searchTerm)
        {
            SearchTerm = searchTerm;
            var userId = await GetCurrentUserId();
            Tasks = (await service.GetAll(userId));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                Tasks = Tasks.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            if (DisplayOnlyTodaysTasks)
            {
                Tasks = Tasks.Where(t => t.Date.Date == System.DateTime.Now.Date);
            }

            if (DisplayDoneTasks)
            {
                Tasks = Tasks.Where(t => t.Done == true);
            }
            else
            {
                Tasks = Tasks.Where(t => t.Done == false);
            }

            StateHasChanged();                
        }

        public async void SortByColumn(string columnName)
        {
            SortingColumn = columnName;
            switch(SortingColumn)
            {
                case "Date":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Date) : Tasks.OrderByDescending(t => t.Date);
                    break;
                case "Priority":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Priority) : Tasks.OrderByDescending(t => t.Priority);
                    break;
                case "Title":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Title) : Tasks.OrderByDescending(t => t.Title);
                    break;
                case "Done":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Done) : Tasks.OrderByDescending(t => t.Done);
                    break;
                case "Category":
                    Tasks = SortingDirection == "Asc" ? Tasks.OrderBy(t => t.Category?.Name) : Tasks.OrderByDescending(t => t.Category?.Name);
                    break;
            }
                       
            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
            StateHasChanged();
        }

        public async void ShowTasksFromToday()
        {
            DisplayOnlyTodaysTasks = !DisplayOnlyTodaysTasks;
            FilterList(SearchTerm);
            SortByColumn(SortingColumn);

            StateHasChanged();
        }

        public async void ShowTasksDone()
        {
            DisplayDoneTasks = !DisplayDoneTasks;
            var userId = await GetCurrentUserId();
            Tasks = (await service.GetAll(userId));

            if (DisplayDoneTasks)
            {
                Tasks = Tasks.Where(t => t.Done == true);
            }
            else
            {
                Tasks = Tasks.Where(t => t.Done == false);
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
