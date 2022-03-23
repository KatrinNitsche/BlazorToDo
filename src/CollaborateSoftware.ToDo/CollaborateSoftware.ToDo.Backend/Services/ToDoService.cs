using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
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

        public async Task<IEnumerable<ToDoListEntry>> GetAll()
        {
            return toDoListentryRepository.GetAll();
        }

        public async Task<ToDoListEntry> Add(ToDoListEntry toDoListEntry)
        {
            toDoListentryRepository.Add(toDoListEntry);
            toDoListentryRepository.Commit();
            return toDoListEntry;
        }

        public void ToggleDone(int id)
        {
            ToDoListEntry entry = toDoListentryRepository.GetById(id);
            if (entry != null)
            {
                entry.Done = !entry.Done;
                toDoListentryRepository.Commit();
            }
        }

        public async Task<ToDoListEntry> GetById(int idNumber)
        {
            return toDoListentryRepository.GetById(idNumber);
        }

        public async Task<ToDoListEntry> Update(ToDoListEntry toDoListEntry)
        {
            return toDoListentryRepository.Update(toDoListEntry);
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
