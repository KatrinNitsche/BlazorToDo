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
    public partial class EditNoteDialog
    {
        public IEnumerable<NotesEntry> NotesList { get; set; }

        public NotesEntry Note { get; set; } = new NotesEntry { Title = "New Note" };

        public IEnumerable<Category> CategoryList { get; set; }

        [Inject]
        public INotesService service { get; set; }

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

        public string CurrentParentId { get; set; }

        public async void Show()
        {
            var userId = await GetCurrentUserId();
            CategoryList = await categoryService.GetAll(userId);
            NotesList = (await service.GetAll(userId));
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
            CurrentParentId = "";
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public async void Show(int id)
        {
            var userId = await GetCurrentUserId();
            CategoryList = await categoryService.GetAll(userId);
            NotesList = (await service.GetAll(userId));
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();           
            Note = await service.GetById(id);
            CurrentParentId = Note.ParentNoteId.ToString();
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
            Note = new NotesEntry { Title = "New Note" };
        }

        protected async Task HandleValidSubmit()
        {
            Note.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            Note.Updated = DateTime.Now;
            if (string.IsNullOrEmpty(CurrentParentId))
            {
                Note.ParentNoteId = null;
            }
            else
            {
                Note.ParentNoteId = NotesList.FirstOrDefault(n => n.Id == int.Parse(CurrentParentId)).Id;
            }

            var result = await service.Update(Note);
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
