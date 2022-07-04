using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using CollaborateSoftware.MyLittleHelpers.Backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private CategoryRepository categoryRepository;
        private ToDoListEntryRepository toDoListentryRepository;

        public CategoryService(DataContext context)
        {
            categoryRepository = new CategoryRepository(context);
            toDoListentryRepository = new ToDoListEntryRepository(context);
        }

        public async Task<IEnumerable<Category>> GetAll() => categoryRepository.GetAll();

        public async Task<Category> GetById(int idNumber) => categoryRepository.GetById(idNumber);

        public async Task<Category> Update(Category category) => categoryRepository.Update(category);

        public async Task<bool> Remove(int id)
        {
            try
            {
                // Category is still used for at least one todo
                if (toDoListentryRepository.GetAll().Any(t => t.Category?.Id == id))
                {
                    return false;
                }

                categoryRepository.Remove(id);
                categoryRepository.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Category> Add(Category newCategory)
        {
            categoryRepository.Add(newCategory);
            categoryRepository.Commit();
            return newCategory;
        }
    }
}
