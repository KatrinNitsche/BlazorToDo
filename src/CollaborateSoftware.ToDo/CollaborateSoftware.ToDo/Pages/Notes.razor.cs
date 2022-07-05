using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Components;
using Microsoft.AspNetCore.Components;
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

        [Inject]
        public INotesService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            NotesList = (await service.GetAll());
        }

        protected AddNoteDialog AddNoteDialog { get; set; }
        protected EditNoteDialog EditNoteDialog { get; set; }

        protected void AddToDoEntry()
        {
            AddNoteDialog.Show();
        }

        protected void EditToDoEntry(int id)
        {
            EditNoteDialog.Show(id);
        }

        public async void AddNotentryDialog_OnDialogClose()
        {
            NotesList = (await service.GetAll());
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
                NotesList = (await service.GetAll());
            }
            else
            {
                NotesList = NotesList.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()) || 
                                                 t.Text.ToLower().Contains(searchTerm.ToLower()));
            }

            StateHasChanged();
        }
    }
}
