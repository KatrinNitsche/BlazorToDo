using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoListEntry>> GetAll();
        Task<ToDoListEntry> Add(ToDoListEntry newTask);
        void ToggleDone(int id);
        Task<ToDoListEntry> GetById(int idNumber);
        Task<ToDoListEntry> Update(ToDoListEntry toDoListEntry);
        Task<bool> Remove(int id);
        Task<bool> Export(ToDoExportSettings exportSettings);
    }
}