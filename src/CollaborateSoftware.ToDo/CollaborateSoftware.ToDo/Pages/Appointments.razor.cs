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
    public partial class Appointments
    {
        public IEnumerable<Appointment> AppointmentList { get; set; }

        protected AddAppointmentDialog AddAppointmentDialog { get; set; }
        protected EditAppointmentDialog EditAppointmentDialog { get; set; }

        public string SearchTerm { get; set; }
        public string SortingColumn { get; set; }
        public string SortingDirection { get; set; } = "Asc";
        public string CurrentCategoryId { get; set; }
        public bool DisplayOnlyTodaysEntries { get; set; }
        public DateTime FilterFromDate { get; set; } 
        public DateTime FilterToDate { get; set; } 

        public IEnumerable<Category> CategoryList { get; set; }
        
        protected PdfCreationDialog PdfCreationDialog { get; set; }

        [Inject]
        public IAppointmentService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        public void PdfExport()
        {
            PdfCreationDialog.Show();
        }

        protected async override Task OnInitializedAsync()
        {
            AppointmentList = (await service.GetAll());
            CategoryList = await categoryService.GetAll();

            FilterFromDate = DateTime.Now;
            FilterToDate = DateTime.Now.AddDays(7);
            StateHasChanged();
        }

        protected void AddEntry()
        {
            AddAppointmentDialog.Show();
        }

        public async void AddAppointmentDialog_OnDialogClose()
        {
            AppointmentList = (await service.GetAll());
            StateHasChanged();
        }

        public async void SortByColumn(string columnName)
        {
            switch (columnName)
            {
                case "Date":
                    AppointmentList = SortingDirection == "Asc" ? AppointmentList.OrderBy(t => t.Date) : AppointmentList.OrderByDescending(t => t.Date);
                    break;
                case "Priority":
                    AppointmentList = SortingDirection == "Asc" ? AppointmentList.OrderBy(t => t.Priority) : AppointmentList.OrderByDescending(t => t.Priority);
                    break;
                case "Title":
                    AppointmentList = SortingDirection == "Asc" ? AppointmentList.OrderBy(t => t.Title) : AppointmentList.OrderByDescending(t => t.Title);
                    break;              
                case "Category":
                    AppointmentList = SortingDirection == "Asc" ? AppointmentList.OrderBy(t => t.Category?.Name) : AppointmentList.OrderByDescending(t => t.Category?.Name);
                    break;
            }

            SortingColumn = columnName;
            SortingDirection = SortingDirection == "Asc" ? "Desc" : "Asc";
            StateHasChanged();
        }

        protected void EditEntry(int id)
        {
            EditAppointmentDialog.Show(id);
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

        public async void FilterList(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                AppointmentList = (await service.GetAll());
            }
            else
            {
                AppointmentList = AppointmentList.Where(t => t.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            StateHasChanged();
        }

        public async void FilterByCategory(string selectedId)
        {            
            AppointmentList = (await service.GetAll());           
            if (selectedId != "All")
            {               
                AppointmentList = AppointmentList.Where(t => t.Category.Id == int.Parse(selectedId));
            }

            StateHasChanged();
        }

        public async void ShowEntriesFromToday()
        {
            DisplayOnlyTodaysEntries = !DisplayOnlyTodaysEntries;

            AppointmentList = (await service.GetAll());
            if (DisplayOnlyTodaysEntries)
            {
                AppointmentList = AppointmentList.Where(t => t.Date.Date == System.DateTime.Now.Date);
            }        

            StateHasChanged();
        }

        public async void FilterListByFromDate(string FromDate)
        {
            FilterFromDate = DateTime.Parse(FromDate);
            AppointmentList = (await service.GetAll());
            AppointmentList = AppointmentList.Where(t => (FilterFromDate == DateTime.MinValue || t.Date.Date >= FilterFromDate) && (FilterToDate == DateTime.MinValue || t.Date.Date <= FilterToDate));

            StateHasChanged();
        }

        public async void FilterListByTodate(string  toDate)
        {
            FilterToDate = DateTime.Parse(toDate);
            AppointmentList = (await service.GetAll());
            AppointmentList = AppointmentList.Where(t => (FilterToDate == DateTime.MinValue || t.Date.Date <= FilterToDate) && (FilterFromDate == DateTime.MinValue || t.Date.Date >= FilterFromDate));

            StateHasChanged();
        }
    }
}
