
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class BaseRepository<TKey, TEntity> : IRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        protected ApplicationContext db;

        public BaseRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public virtual void Creat(TEntity item)
        {
            if (item != null)
                db.Set<TEntity>().Add(item);
        }

        public virtual void Delete(TKey id)
        {
            var entity = db.Set<TEntity>().Find(id);
            if (entity != null)
                db.Set<TEntity>().Remove(entity);
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return db.Set<TEntity>().Where(predicate).ToList();
        }

        public virtual TEntity? Get(TKey id)
        {
            var entity = db.Set<TEntity>().Find(id);
            return entity ?? default;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return db.Set<TEntity>();
        }

        public virtual void Update(TEntity item)
        {
            db.Set<TEntity>().Update(item);
        }

        public virtual void UpdateRange(List<TEntity> items)
        {
            db.Set<TEntity>().UpdateRange(items);
        }
    }
}
