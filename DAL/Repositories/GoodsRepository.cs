using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class GoodsRepository : BaseRepository<int,Goods>
    {
        public GoodsRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<Goods> Find(Func<Goods, bool> predicate)
        {
            return db.Set<Goods>().AsNoTracking().Where(predicate);
        }

        public override IEnumerable<Goods> GetAll()
        {
            return db.Set<Goods>().Include(g=>g.QueueForPurchase).Include(g=>g.GoodsInStock).AsNoTracking();
        }
    }
}
