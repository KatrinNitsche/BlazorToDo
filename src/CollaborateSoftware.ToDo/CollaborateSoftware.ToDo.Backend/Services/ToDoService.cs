using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class ToDoService : IToDoService
    {
        private ToDoListEntryRepository toDoListentryRepository;

        public ToDoService(DataContext context)
        {
            toDoListentryRepository = new ToDoListEntryRepository(context);
        }

        public async Task<IEnumerable<ToDoListEntry>> GetAll() => toDoListentryRepository.GetAll();
        public async Task<ToDoListEntry> GetById(int idNumber) => toDoListentryRepository.GetById(idNumber);
        public async Task<ToDoListEntry> Update(ToDoListEntry toDoListEntry) => toDoListentryRepository.Update(toDoListEntry);

        public async Task<ToDoListEntry> Add(ToDoListEntry toDoListEntry)
        {
            toDoListentryRepository.Add(toDoListEntry);
            toDoListentryRepository.Commit();
            return toDoListEntry;
        }

        public async void ToggleDone(int id)
        {
            ToDoListEntry entry = toDoListentryRepository.GetById(id);
            if (entry != null)
            {
                entry.Done = !entry.Done;
                if (entry.RepetitionType != RepetitionType.None && entry.Done)
                {
                    var nextEntry = new ToDoListEntry()
                    {
                        Done = false,
                        Description = entry.Description,
                        Title = entry.Title,
                        Priority = entry.Priority,
                        RepetitionType = entry.RepetitionType,
                        Date = GetNextDateFor(entry.Date, entry.RepetitionType)
                    };

                    await Add(nextEntry);
                }
                
                toDoListentryRepository.Commit();
            }
        }

        private DateTime GetNextDateFor(DateTime date, RepetitionType repetitionType)
        {
            switch (repetitionType)
            {
                case RepetitionType.None:
                    return date;
                case RepetitionType.Daily:
                    return date.AddDays(1);
                case RepetitionType.Weekly:
                    return date.AddDays(7);
                case RepetitionType.TwoWeekly:
                    return date.AddDays(14);
                case RepetitionType.Monthly:
                    return date.AddMonths(1);
                case RepetitionType.Quarterly:
                    return date.AddMonths(3);
                case RepetitionType.Halfyearly:
                    return date.AddMonths(6);
                case RepetitionType.Yearly:
                    return date.AddYears(1);
                default:
                    return date;
            }
        }

        public async Task<bool> Remove(int id)
        {
            try
            {
                toDoListentryRepository.Remove(id);
                toDoListentryRepository.Commit();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }

        }
    }
}
