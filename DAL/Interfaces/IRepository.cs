using DAL.Entities;
using System.Linq.Expressions;

namespace DAL.Interfaces
{
    public interface IRepository<TKey, TEntity> where TEntity : BaseEntity<TKey>
    {
        IEnumerable<TEntity> GetAll();
        TEntity? Get(TKey id);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, Boolean>> predicate);
        void Creat(TEntity item);
        void Update(TEntity item);
        void UpdateRange(List<TEntity> items);
        void Delete(TKey id);
    }
}
