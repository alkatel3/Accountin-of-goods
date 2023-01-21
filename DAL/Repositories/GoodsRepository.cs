using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class GoodsRepository : BaseRepository<int,Goods>
    {
        public GoodsRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<Goods> Find(Expression<Func<Goods, bool>> predicate)
        {
            return db.Set<Goods>().AsNoTracking().Where(predicate);
        }

        public override IEnumerable<Goods> GetAll()
        {
            return db.Set<Goods>()
                .Include(g => g.QueueForPurchase)
                .Include(g => g.GoodsInStock)
                .AsNoTracking();
        }

        public override Goods? Get(int id)
        {
            var entity = db.Set<Goods>()
                .AsNoTracking()
                .Include(g => g.GoodsInStock)
                .Include(g => g.QueueForPurchase)
                .FirstOrDefault(g=>g.Id==id);
            return entity ?? default;
        }
    }
}
