using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class AddNoteDialog
    {
        public IEnumerable<NotesEntry> NotesList { get; set; }

        public NotesEntry Note { get; set; } = new NotesEntry { Title = "New Note"};

        public IEnumerable<Category> CategoryList { get; set; }

        [Inject]
        public INotesService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }

        public string CurrentCategoryId { get; set; }

        public string CurrentParentId { get; set; }

        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            NotesList = (await service.GetAll());
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
            CurrentParentId = "";
            ResetDialog();
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
            if (string.IsNullOrEmpty(CurrentParentId))
            {
                Note.ParentNoteId = null;
            }
            else
            {
                Note.ParentNoteId = NotesList.FirstOrDefault(n => n.Id == int.Parse(CurrentParentId)).Id;
            }

            var result = await service.Add(Note);
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
    }
}
