using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Components
{
    public partial class PdfNotesCreationDialog
    {
        public PdfNotesSettings PdfNotesSettings { get; set; } = new PdfNotesSettings() { CategoryId = "All" };
        
        public IEnumerable<Category> CategoryList { get; set; }

        public IEnumerable<NotesEntry> NotesList { get; set; }

        [Inject]
        public IPdfCreator pdfCreator { get; set; }

        [Inject]
        public INotesService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        public bool ShowDialog { get; set; }

        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            PdfNotesSettings = new PdfNotesSettings() { CategoryId = "All" };            
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            NotesList = (await service.GetAll());

            if (PdfNotesSettings.CategoryId != "All")
            {
                NotesList = NotesList.Where(n => n.Category.Id == int.Parse(PdfNotesSettings.CategoryId)).ToList();
            }

            var result = pdfCreator.CreateNotesPdf(NotesList.ToList(), PdfNotesSettings.NotesTitle);
            if (result != null)
            {
                toastService.ShowSuccess("Pdf document was created");
                ShowDialog = false;
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to create pdf document.");
            }
        }
    }
}
