using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class NotesRepository : BaseRepository<NotesEntry>
    {
        public NotesRepository(DataContext context) : base(context) { }

        public override IEnumerable<NotesEntry> GetAll() => Context.Notes.Include(c => c.Category);

        public override IQueryable<NotesEntry> GetAll(bool asyn = true) => Context.Notes.Include(c => c.Category);

        public override NotesEntry GetById(int id) => Context.Notes.FirstOrDefault(n => n.Id == id);
        public override NotesEntry ToggleActive(int id)
        {
            throw new NotImplementedException();
        }
    }
}
