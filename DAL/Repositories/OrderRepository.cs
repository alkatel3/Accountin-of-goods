using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class OrderRepository : BaseRepository<int, Order>
    {
        public OrderRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<Order> Find(Expression<Func<Order, bool>> predicate)
        {
            return db.Set<Order>()

                //.Include(o => o.Goods)
                //.ThenInclude(g => g.GoodsInStock)
                .Where(predicate).AsNoTracking();
        }

        public override IEnumerable<Order> GetAll()
        {
            return db.Set<Order>().Include(o => o.Goods).AsNoTracking();
        }

        public override Order? Get(int id)
        {
            var client = db.Set<Order>()
                .Include(o => o.Goods)
                .ThenInclude(g => g.GoodsInStock)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == id);
            return client ?? default;
        }
    }
}
