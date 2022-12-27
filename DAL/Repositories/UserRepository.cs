using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<int, User>
    {
        public UserRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<User> Find(Func<User, bool> predicate)
        {
            var users = db.Set<User>()
                .Include(c => c.OrderList)
                .ThenInclude(ol => ol.Orders)
                .ThenInclude(o => o.Goods)
                .AsNoTracking()
                .Where(predicate);
            return users;
        }

        public override User? Get(int id)
        {
            var client = db.Set<User>().AsNoTracking()
                .Include(c => c.OrderList)
                .ThenInclude(ol => ol.Orders)
                .ThenInclude(o => o.Goods)
                .FirstOrDefault(c => c.Id == id || c.PhoneNumber == id);
            return client ?? default;
        }
    }
}
