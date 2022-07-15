using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IHabitService
    {
        Task<IEnumerable<Habit>> GetAll();
        Task<Habit> Add(Habit habit);
        Task<Habit> GetById(int idNumber);
        Task<Habit> Update(Habit habit);
        Task<bool> Remove(int id);
        bool DoneToday(int id);
        bool DoneOnDay(int id, DateTime date);
    }
}
