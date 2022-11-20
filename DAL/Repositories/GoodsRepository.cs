using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class GoodsRepository:IRepository<Goods>
    {
        private ApplicationContext db;

        public GoodsRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Goods> GetAll()
        {
            return db.Goods.Include(g => g.Category);
        }

        public Goods? Get(int id)
        {
            var goods = db.Goods.Find(id);
            return goods ?? default;
        }

        public void Creat(Goods goods)
        {
            db.Goods.Add(goods);
        }

        public void Update(Goods goods)
        {
            db.Goods.Update(goods);
        }

        public void Delete(int id)
        {
            Goods? goods = db.Goods.Find(id);
            if (goods != null)
            db.Goods.Remove(goods);
        }

        public IEnumerable<Goods> Find(Func<Goods, bool> predicate)
        {
            return db.Goods.Where(predicate).ToList();
        }
    }
}
