using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetEntry>> GetAll(Guid userId);
        Task<List<BudgetEntry>> GetAll(Guid userId, DateTime from, DateTime to);
        Task<BudgetEntry> Add(BudgetEntry newEntry);

        Task<BudgetEntry> GetById(int idNumber);
        Task<BudgetEntry> Update(BudgetEntry entry);
        Task<bool> Remove(int id);
    }
}
