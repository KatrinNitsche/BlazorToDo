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
    public partial class EditAppointmentDialog
    {
        public Appointment AppointmentEntry { get; set; } = new Appointment { Title = string.Empty, Date = DateTime.Now, Priority = Priority.Middle };
        public IEnumerable<Category> CategoryList { get; set; }

        [Inject]
        public IAppointmentService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }
       
        [Inject]
        public ICategoryService categoryService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public bool ShowDialog { get; set; }
        
        public string CurrentCategoryId { get; set; }

        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        public async void Show(int id)
        {
            CategoryList = (await categoryService.GetAll());
            AppointmentEntry = await service.GetById(id);          
            CurrentCategoryId = AppointmentEntry.Category.Id.ToString();
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
            AppointmentEntry = new Appointment { Title = string.Empty, Date = DateTime.Now, Priority = Priority.Middle };
        }

        protected async Task HandleValidSubmit()
        {
            AppointmentEntry.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            var result = await service.Update(AppointmentEntry);
            if (result != null)
            {
                ShowDialog = false;
                toastService.ShowSuccess("Entry was updated successfully");

                await CloseEventCallback.InvokeAsync(true);
                StateHasChanged();
            }
            else
            {
                toastService.ShowError("Unable to update entry.");
            }
        }
    }
}
