using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class OrderListRepository : BaseRepository<int,OrderList>
    {
        public OrderListRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<OrderList> GetAll()
        {
            return db.Set<OrderList>().Include(ol => ol.Orders);
        }
    }
}
