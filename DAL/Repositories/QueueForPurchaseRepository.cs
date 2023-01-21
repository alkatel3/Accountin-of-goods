using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class QueueForPurchaseRepository : BaseRepository<int, QueueForPurchase>
    {
        public QueueForPurchaseRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<QueueForPurchase> GetAll()
        {
            return db.Set<QueueForPurchase>()
                .Include(qp => qp.Goods)
                .ThenInclude(g => g.GoodsInStock).AsNoTracking();
        }

        public override IEnumerable<QueueForPurchase> Find(Expression<Func<QueueForPurchase, bool>> predicate)
        {
            var GoodsInStock = db.Set<QueueForPurchase>().AsNoTracking().Where(predicate);
            return GoodsInStock;
        }

        public override QueueForPurchase? Get(int id)
        {
            var client = db.Set<QueueForPurchase>().AsNoTracking()
                .Include(q => q.Goods)
                .ThenInclude(g => g.GoodsInStock)
                .FirstOrDefault(c => c.Id == id);
            return client ?? default;
        }
    }
}
