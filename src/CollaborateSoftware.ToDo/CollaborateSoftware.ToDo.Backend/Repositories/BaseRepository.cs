using CollaborateSoftware.MyLittleHelpers.Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Repositories
{
    public abstract class BaseRepository<TEntity>
         where TEntity : class
    {
        protected DataContext Context = null;

        public BaseRepository(DataContext context)
        {
            Context = context;
        }

        public abstract TEntity GetById(int id);
        public abstract IEnumerable<TEntity> GetAll();
        public abstract IQueryable<TEntity> GetAll(bool asyn = true);
        public abstract TEntity ToggleActive(int id);

        public TEntity Add(TEntity entry)
        {            
            Context.Set<TEntity>().Add(entry);
            return entry;
        }

        public void Remove(int id)
        {
            var set = Context.Set<TEntity>();
            var entity = set.Find(id);
            Context.Remove(entity);
            Commit();
        }

        public TEntity Update(TEntity entry)
        {
            Context.Entry(entry).State = EntityState.Modified;
            Context.SaveChanges();
            return entry;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }
    }
}
