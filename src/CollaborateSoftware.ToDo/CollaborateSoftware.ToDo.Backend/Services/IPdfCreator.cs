using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IPdfCreator
    {
        Task<bool> CreateDailySheet(List<ToDoListEntry> todos, List<string> priorities, string forTomorrow, string note);
        Task<bool> CreateWeekPlan(List<ToDoListEntry> todos, List<string> priorities, DateTime firstDayOfWeek);
        Task<bool> CreateMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth);
        Task<bool> CreateYearPlan(DateTime firstDayofYear);
    }
}
