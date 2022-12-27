using DAL.Interfaces;
using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderRepository : BaseRepository<int, Order>
    {
        public OrderRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<Order> Find(Func<Order, bool> predicate)
        {
            return db.Set<Order>().AsNoTracking().Where(predicate);
        }

        public override IEnumerable<Order> GetAll()
        {
            return db.Set<Order>().Include(o => o.Goods).AsNoTracking();
        }
    }
}
