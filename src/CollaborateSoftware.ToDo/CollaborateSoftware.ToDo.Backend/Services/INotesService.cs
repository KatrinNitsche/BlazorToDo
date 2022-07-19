using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface INotesService
    {
        Task<IEnumerable<NotesEntry>> GetAll(Guid userId);
        Task<NotesEntry> Add(NotesEntry newNote);
        Task<NotesEntry> GetById(int idNumber);
        Task<NotesEntry> Update(NotesEntry notesEntry);
        Task<bool> Remove(int id);
    }
}
