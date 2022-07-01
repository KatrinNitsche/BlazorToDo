using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class ToDoListEntryRepository : BaseRepository<ToDoListEntry>
    {
        public ToDoListEntryRepository(DataContext context) : base(context) { }

        public override IEnumerable<ToDoListEntry> GetAll()
        {
            return Context.ToDoList;
        }

        public override IQueryable<ToDoListEntry> GetAll(bool asyn = true) => Context.ToDoList;

        public override ToDoListEntry GetById(int id)
        {
            return Context.ToDoList.FirstOrDefault(s => s.Id == id);
        }

        public override ToDoListEntry ToggleActive(int id)
        {
            var entry = GetById(id);          
            return entry;
        }
    }
}
