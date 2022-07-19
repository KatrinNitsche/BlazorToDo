using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class HabitService : IHabitService
    {
        private HabitRepository habitRepository;

        public HabitService(DataContext context)
        {
            habitRepository = new HabitRepository(context);
        }

        public async Task<IEnumerable<Habit>> GetAll(Guid userId) => habitRepository.GetAll().Where(x => x.UserId == userId);

        public async Task<Habit> Add(Habit newTask)
        {
            if (habitRepository.GetAll().Any(h => h.Title == newTask.Title)) return null;

            habitRepository.Add(newTask);
            habitRepository.Commit();
            return newTask;
        }

        public async Task<Habit> GetById(int idNumber) => habitRepository.GetAll().First(h => h.Id == idNumber);

        public async Task<Habit> Update(Habit habit) => habitRepository.Update(habit);

        public async Task<bool> Remove(int id)
        {
            try
            {
                habitRepository.Remove(id);
                habitRepository.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DoneToday(int id)
        {
            return DoneOnDay(id, DateTime.Now);
        }

        public bool DoneOnDay(int id, DateTime date)
        {
            var habitEntry = habitRepository.GetById(id);
            if (habitEntry == null) return false;

            if (habitEntry.Done.Any(d => d.Date.Date == date.Date)) return false; // already done today

            habitEntry.Done.Add(new HabitDone() { Date = DateTime.Now });
            if (habitRepository.Update(habitEntry) == null) return false;

            return true;
        }
    }
}
