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
    public partial class AddHabitDialog
    {
        public Habit Habit { get; set; } = new Habit();
        public IEnumerable<Category> CategoryList { get; set; }
        public bool ShowDialog { get; set; }

        [Inject]
        public IHabitService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        [Inject]
        public ICategoryService categoryService { get; set; }

        [Parameter]
        public EventCallback<bool> CloseEventCallback { get; set; }

        public string CurrentCategoryId { get; set; }

        public async void Show()
        {
            CategoryList = await categoryService.GetAll();
            CurrentCategoryId = CategoryList.FirstOrDefault(c => c.Name == "None")?.Id.ToString();
            ResetDialog();
            ShowDialog = true;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            Habit = new Habit();
        }

        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        protected async Task HandleValidSubmit()
        {
            Habit.Category = CategoryList.FirstOrDefault(c => c.Id == int.Parse(CurrentCategoryId));
            var result = await service.Add(Habit);
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
