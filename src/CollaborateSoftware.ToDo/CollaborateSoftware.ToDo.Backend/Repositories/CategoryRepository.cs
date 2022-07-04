using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public class CategoryRepository : BaseRepository<Category>
    {
        public CategoryRepository(DataContext context) : base(context) { }

        public override IEnumerable<Category> GetAll() => Context.Category;

        public override IQueryable<Category> GetAll(bool asyn = true) => Context.Category;

        public override Category GetById(int id) => Context.Category.First(c => c.Id == id);

        public override Category ToggleActive(int id)
        {
            var entry = GetById(id);
            return entry;
        }
    }
}
