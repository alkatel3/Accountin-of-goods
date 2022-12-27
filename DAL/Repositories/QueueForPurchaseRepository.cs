using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public override IEnumerable<QueueForPurchase> Find(Func<QueueForPurchase, bool> predicate)
        {
            var GoodsInStock = db.Set<QueueForPurchase>().AsNoTracking().Where(predicate);
            return GoodsInStock;
        }
    }
}
