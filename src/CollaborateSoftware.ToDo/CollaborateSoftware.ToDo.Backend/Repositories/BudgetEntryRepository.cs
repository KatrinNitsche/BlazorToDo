using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class BudgetEntryRepository : BaseRepository<BudgetEntry>
    {
        public BudgetEntryRepository(DataContext context) : base(context) { }

        public override IEnumerable<BudgetEntry> GetAll() => Context.BudgetEntries.Include(c => c.Category);
        public override IQueryable<BudgetEntry> GetAll(bool asyn = true) => Context.BudgetEntries.Include(c => c.Category);
        public override BudgetEntry GetById(int id) => Context.BudgetEntries.FirstOrDefault(b => b.Id == id);
        public override BudgetEntry ToggleActive(int id)
        {
            throw new NotImplementedException();
        }
    }
}
