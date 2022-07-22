using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class BudgetService : IBudgetService
    {
        private BudgetEntryRepository budgetEntryRepository;

        public BudgetService(DataContext context) 
        {
            budgetEntryRepository = new BudgetEntryRepository(context);
        }

        public async Task<BudgetEntry> Add(BudgetEntry newEntry)
        {           
            budgetEntryRepository.Add(newEntry);
            budgetEntryRepository.Commit();
            return newEntry;
        }

        public async Task<IEnumerable<BudgetEntry>> GetAll(Guid userId) => budgetEntryRepository.GetAll().Where(b => b.UserId == userId);

        public async Task<List<BudgetEntry>> GetAll(Guid userId, DateTime from, DateTime to)
        {
            var data = budgetEntryRepository.GetAll().Where(b => b.UserId == userId).Where(t => t.BudgetDate >= from && t.BudgetDate <= to);
            return data.ToList();
        }

        public async Task<BudgetEntry> GetById(int idNumber) => budgetEntryRepository.GetById(idNumber);

        public async Task<bool> Remove(int id)
        {
            try
            {
                budgetEntryRepository.Remove(id);
                budgetEntryRepository.Commit();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        public async Task<BudgetEntry> Update(BudgetEntry entry) => budgetEntryRepository.Update(entry);
    }
}
