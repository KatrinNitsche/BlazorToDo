using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class ToDoService : IToDoService
    {
        private ToDoListEntryRepository toDoListentryRepository;
        private CategoryRepository categoryRepository;

        public ToDoService(DataContext context)
        {
            toDoListentryRepository = new ToDoListEntryRepository(context);
            categoryRepository = new CategoryRepository(context);
        }

        public async Task<IEnumerable<ToDoListEntry>> GetAll() => toDoListentryRepository.GetAll();

        public async Task<List<ToDoListEntry>> GetAll(DateTime from, DateTime to)
        {
            var data = toDoListentryRepository.GetAll().ToList();
            var repeatingTasks = toDoListentryRepository.GetAll().Where(t => t.RepetitionType != RepetitionType.None && t.Done == false).Distinct();

            var result = new List<ToDoListEntry>();
            foreach (var entry in data)
            {
                result.Add(new ToDoListEntry()
                {
                    Category = entry.Category,
                    Date = entry.Date,
                    Description = entry.Description,
                    Done = entry.Done,
                    Id = entry.Id,
                    Priority = entry.Priority,
                    RepetitionType = entry.RepetitionType,
                    Title = entry.Title 
                });
            }          

            foreach (var task in repeatingTasks)
            {
                if (task.Date > to) continue;

                var nextDate = task.Date;
                while (nextDate <= to)
                {
                    nextDate = GetNextDateFor(nextDate, task.RepetitionType);

                    result.Add(new ToDoListEntry()
                    {
                        Category = task.Category,
                        Date = nextDate,
                        Description = task.Description,
                        Done = false,
                        Id = -1,
                        Priority = task.Priority,
                        RepetitionType = task.RepetitionType,
                        Title = task.Title
                    });
                }
            }
                        
            return result.Where(t => t.Date >= from && t.Date <= to).ToList();
        }

        public async Task<ToDoListEntry> GetById(int idNumber) => toDoListentryRepository.GetById(idNumber);
        public async Task<ToDoListEntry> Update(ToDoListEntry toDoListEntry) => toDoListentryRepository.Update(toDoListEntry);

        public async Task<ToDoListEntry> Add(ToDoListEntry toDoListEntry)
        {
            if (toDoListentryRepository.GetAll().Any(t => t.Title == toDoListEntry.Title && t.Done == false)) return null;

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
                        Date = GetNextDateFor(entry.Date, entry.RepetitionType),
                        Category = entry.Category
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

        public async Task<bool> Export(ToDoExportSettings exportSettings)
        {
            try
            {
                var dataToExport = FilterToDos(exportSettings).ToList();
                var filePath = @"C:\tmp\todos.csv";

                // export
                if (dataToExport.Count == 0) return false;

                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader(typeof(ToDoListEntry));                   
                    csv.NextRecord();
                    csv.WriteRecords(dataToExport);
                    csv.Flush();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private IEnumerable<ToDoListEntry> FilterToDos(ToDoExportSettings exportSettings)
        {
            var allTasks = toDoListentryRepository.GetAll();
            if (exportSettings.Today)
            {
                allTasks = allTasks.Where(t => t.Date.Date == DateTime.Now.Date);
            }
            else if (exportSettings.FromDate != DateTime.MinValue && exportSettings.ToDate != DateTime.MinValue)
            {
                allTasks = allTasks.Where(t => t.Date.Date <= exportSettings.ToDate && t.Date.Date >= exportSettings.ToDate);
            }

            if (exportSettings.Open)
            {
                allTasks = allTasks.Where(t => t.Done == exportSettings.Open);
            }

            if (!string.IsNullOrEmpty(exportSettings.CategoryId))
            {
                allTasks = allTasks.Where(t => t.Category == categoryRepository.GetById(int.Parse(exportSettings.CategoryId)));
            }

            allTasks = allTasks.OrderBy(t => t.Date).ThenBy(t => t.Title);

            return allTasks;
        }
    }
}
