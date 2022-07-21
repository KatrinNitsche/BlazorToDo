using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IPdfCreator
    {
        Task<bool> CreateDailySheet(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, string forTomorrow, string note, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<bool> CreateWeekPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, DateTime firstDayOfWeek, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<bool> CreateMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<bool> CreateYearPlan(DateTime firstDayofYear);
        Task<bool> CreateNotesPdf(List<NotesEntry> notesEntries, string notesTitle);
    }
}
