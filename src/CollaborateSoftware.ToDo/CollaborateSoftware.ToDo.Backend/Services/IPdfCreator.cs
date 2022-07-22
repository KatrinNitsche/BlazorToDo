using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IPdfCreator
    {
        Task<byte[]> CreateDailySheet(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, string forTomorrow, string note, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<byte[]> CreateWeekPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, DateTime firstDayOfWeek, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<byte[]> CreateMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth, bool includeFinance, List<BudgetEntry> financeInformation);
        Task<byte[]> CreateYearPlan(DateTime firstDayofYear);
        Task<byte[]> CreateNotesPdf(List<NotesEntry> notesEntries, string notesTitle);

        string GetHtmlCodeForNotes(List<NotesEntry> notesEntries, string notesTitle);
        string GetChildNoteHtml(List<NotesEntry> notesEntries, int parentId);
        string GetHtmlCodeForMonthPlan(string focusText, List<string> actionSteps, DateTime firstDayOfMonth, bool includeFinance, List<BudgetEntry> financeInformation);
        string GetHtmlCodeForWeekPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, DateTime firstDayOfWeek, bool includeFinance, List<BudgetEntry> financeInformation);
        string GetHtmlCodeForDayPlan(List<ToDoListEntry> todos, List<Appointment> appointments, List<string> priorities, string forTomorrow, string note, bool includeFinance, List<BudgetEntry> financeInformation);
        string GetHtmlCodeForYearPlan(DateTime firstDayOfYear);
    }
}
