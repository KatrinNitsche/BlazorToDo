using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> Add(Category newTask);
        Task<Category> GetById(int idNumber);
        Task<Category> Update(Category toDoListEntry);
        Task<bool> Remove(int id);
    }
}
