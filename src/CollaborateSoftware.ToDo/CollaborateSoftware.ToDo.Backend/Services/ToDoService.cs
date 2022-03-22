using CollaborateSoftware.ToDo.Backend.Data;
using CollaborateSoftware.ToDo.Backend.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.ToDo.Backend.Services
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
    }
}
