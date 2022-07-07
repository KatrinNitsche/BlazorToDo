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
    public partial class PdfCreationDialog
    {
        public PdfSettings PdfSettings { get; set; } = new PdfSettings() { Type = "Day" };

        public IEnumerable<ToDoListEntry> Tasks { get; set; }

        public bool ShowDialog { get; set; }

        [Inject]
        public IPdfCreator pdfCreator { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public IToDoService toDoService { get; set; }

        public async void Show()
        {
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            PdfSettings = new PdfSettings() { Type = "Day" };
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Tasks = (await toDoService.GetAll());

            if (PdfSettings.Type == "Day")
            {
                Tasks = Tasks.Where(t => t.Date == PdfSettings.Date);

                var priorities = new List<string>();
                if (!string.IsNullOrEmpty(PdfSettings.Priority1)) priorities.Add(PdfSettings.Priority1);
                if (!string.IsNullOrEmpty(PdfSettings.Priority2)) priorities.Add(PdfSettings.Priority2);
                if (!string.IsNullOrEmpty(PdfSettings.Priority3)) priorities.Add(PdfSettings.Priority3);
                if (!string.IsNullOrEmpty(PdfSettings.Priority4)) priorities.Add(PdfSettings.Priority4);

                var result = pdfCreator.CreateDailySheet(Tasks.ToList(), priorities, PdfSettings.ForTomorrow, PdfSettings.Note);
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
}
