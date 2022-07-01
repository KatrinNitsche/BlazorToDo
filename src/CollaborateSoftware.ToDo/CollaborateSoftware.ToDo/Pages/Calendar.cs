using CollaborateSoftware.MyLittleHelpers.Data;
using PublicHoliday;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Pages
{
    public partial class Calendar
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public List<CalendarEntry> CalendarData { get; set; }
        public IList<DateTime> Holidays { get; set; }

        public async void CreateCalendar()
        {
            LoadHolidaysForDateRange();
            CalendarData = new List<CalendarEntry>();

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
