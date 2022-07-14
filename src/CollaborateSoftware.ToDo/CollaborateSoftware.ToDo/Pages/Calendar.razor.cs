using Blazored.Toast.Services;
using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Helper;
using CollaborateSoftware.MyLittleHelpers.Backend.Services;
using CollaborateSoftware.MyLittleHelpers.Components;
using CollaborateSoftware.MyLittleHelpers.Data;
using Microsoft.AspNetCore.Components;
using PublicHoliday;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class Calendar
    {
        public IEnumerable<ToDoListEntry> Tasks { get; set; }        
        public IEnumerable<Appointment> AppointmentList { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Now.Date;
        public DateTime ToDate { get; set; } = DateTime.Now.Date;

        public List<CalendarEntry> CalendarData { get; set; }

        public IList<DateTime> Holidays { get; set; }

        [Inject]
        public IToDoService totoService { get; set; }

        [Inject]
        public IAppointmentService service { get; set; }

        [Inject]
        public IToastService toastService { get; set; }

        protected DateTime CurrentDate { get; set; } = DateTime.Now.Date;

        public async void SelectCurrentDate(DateTime newDate)
        {
            CurrentDate = newDate;
            Tasks = (await totoService.GetAll(CurrentDate, CurrentDate));
            AppointmentList = (await service.GetAll(CurrentDate, CurrentDate));
        }

        public async void CreateCalendar()
        {
            LoadHolidaysForDateRange();
            CalendarData = new List<CalendarEntry>();
            Tasks = (await totoService.GetAll(CurrentDate, CurrentDate));
            if (TimeSpanIsValid())
            {               

                TimeSpan difference = ToDate - FromDate;
                int numberOfDays = difference.Days;

                for (int i = 0; i < numberOfDays; i++)
                {
                    DateTime day = FromDate.AddDays(i);
                    string holidayName = IsHoliday(day);
                    CalendarEntry newEntry = new CalendarEntry() { Date = day };
                    newEntry.CssClass = (holidayName != null || day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday) ? "card bg-light" : "card";
                    newEntry.HolidayName = holidayName;
                    CalendarData.Add(newEntry);
                }
            }       

            StateHasChanged();
        }

        protected async override Task OnInitializedAsync()
        {
            var monday = Tools.MondayBefore(FromDate);
            FromDate = monday;
            ToDate = monday.AddDays(6);
            CreateCalendar();
        }

        private string IsHoliday(DateTime date)
        {
            // ToDo: check list of holidays if day is holiday
            foreach (var day in Holidays)
            {
                if (day == date)
                    return "Holiday";
            }

            return null;
           
        }

        private bool TimeSpanIsValid()
        {
            if (FromDate > ToDate) return false;

            return true;
        }

        private void LoadHolidaysForDateRange()
        {
            Holidays = new UKBankHoliday().PublicHolidays(FromDate.Year);
        }
    }
}
