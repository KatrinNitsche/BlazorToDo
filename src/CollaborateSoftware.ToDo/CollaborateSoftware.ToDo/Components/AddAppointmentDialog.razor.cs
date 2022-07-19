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
    public partial class AddAppointmentDialog
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

        [Inject]
        public SignInManager<IdentityUser> SignInManager { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public UserManager<IdentityUser> userManager { get; set; }

        public bool ShowDialog { get; set; }
        
        public string CurrentCategoryId { get; set; }
        public string Time { get; set; }

        public async void Show()
        {
            var userId = await GetCurrentUserId();
            CategoryList = await categoryService.GetAll(userId);
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
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
            AppointmentEntry = new Appointment { Title = string.Empty, Date = DateTime.Now, Priority = Priority.Middle };
        }

        protected async Task HandleValidSubmit()
        {
            var hour = int.Parse(Time.Substring(0, 2));
            var minute = int.Parse(Time.Substring(3, 2));

            AppointmentEntry.Date = new DateTime(AppointmentEntry.Date.Year, AppointmentEntry.Date.Month, AppointmentEntry.Date.Day, hour, minute, 0);            
            AppointmentEntry.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            AppointmentEntry.Created = DateTime.Now;
            AppointmentEntry.UserId = await GetCurrentUserId();
            var result = await service.Add(AppointmentEntry);
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
