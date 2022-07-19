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
    public partial class Notes
    {
        public IEnumerable<NotesEntry> NotesList { get; set; }
        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";
        public int? CurrentParentNote { get; set; } = null;

        [Inject]
        public INotesService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }
      
        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var userId = await GetCurrentUserId();
            NotesList = (await service.GetAll(userId));
        }

        protected AddNoteDialog AddNoteDialog { get; set; }
        protected EditNoteDialog EditNoteDialog { get; set; }
        protected PdfNotesCreationDialog PdfNotesCreationDialog { get; set; }

        protected void AddToDoEntry()
        {
            AddNoteDialog.Show();
        }

        protected void EditToDoEntry(int id)
        {
            EditNoteDialog.Show(id);
        }

        protected void ShowPdfNotesCreationDialog()
        {
            PdfNotesCreationDialog.Show();
        }

        public async void AddNotentryDialog_OnDialogClose()
        {
            var userId = await GetCurrentUserId();
            NotesList = (await service.GetAll(userId));
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

        public async void SortByColumn(string columnName)
        {
            switch (columnName)
            {
                case "Title":
                    NotesList = SortingDirection == "Asc" ? NotesList.OrderBy(t => t.Title) : NotesList.OrderByDescending(t => t.Title);
                    break;

            }

            SortingColumn = columnName;
            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
            StateHasChanged();
        }

        public async void FilterList(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                var userId = await GetCurrentUserId();
                NotesList = (await service.GetAll(userId));
            }
            else
            {
                NotesList = NotesList.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()) ||
                                                 t.Text.ToLower().Contains(searchTerm.ToLower()));
            }

            StateHasChanged();
        }

        public async void SetParentNote(int? newParentNote)
        {
            CurrentParentNote = newParentNote;
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
