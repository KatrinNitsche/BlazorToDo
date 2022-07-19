using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class NotesService : INotesService
    {
        private NotesRepository notesRepository;

        public NotesService(DataContext context)
        {
            notesRepository = new NotesRepository(context);
        }

        public async Task<NotesEntry> Add(NotesEntry newNote)
        {
            notesRepository.Add(newNote);
            notesRepository.Commit();
            return newNote;
        }

        public async Task<IEnumerable<NotesEntry>> GetAll(Guid userId) => notesRepository.GetAll().Where(x => x.UserId == userId);

        public async Task<NotesEntry> GetById(int idNumber) => notesRepository.GetById(idNumber);

        public async Task<bool> Remove(int id)
        {
            try
            {
                var childNotes = notesRepository.GetAll().Where(n => n.ParentNoteId == id);
                if (childNotes != null && childNotes.Count() > 0) return false;

                notesRepository.Remove(id);
                notesRepository.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<NotesEntry> Update(NotesEntry notesEntry) => notesRepository.Update(notesEntry);
    }
}
