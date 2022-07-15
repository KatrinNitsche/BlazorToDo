using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class HabitRepository : BaseRepository<Habit>
    {
        public HabitRepository(DataContext context):base(context) {}

        public override IEnumerable<Habit> GetAll() => Context.Habits.Include(h => h.Category).Include(h => h.Done);
        public override IQueryable<Habit> GetAll(bool asyn = true) => Context.Habits.Include(h => h.Category).Include(h => h.Done);
        public override Habit GetById(int id) => Context.Habits.First(h => h.Id == id);
        public override Habit ToggleActive(int id)
        {
            throw new NotImplementedException();
        }
    }
}
