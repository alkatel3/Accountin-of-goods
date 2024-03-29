﻿using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DAL.Repositories
{
    public class GoodsInStockRepository : BaseRepository<int, GoodsInStock>
    {
        public GoodsInStockRepository(ApplicationContext context) : base(context) { }

        public override IEnumerable<GoodsInStock> Find(Expression<Func<GoodsInStock, bool>> predicate)
        {
            var GoodsInStock = db.Set<GoodsInStock>().Include(gs => gs.Goods).AsNoTracking().Where(predicate);
            return GoodsInStock;
        }

        public override IEnumerable<GoodsInStock> GetAll()
        {
            return db.Set<GoodsInStock>().Include(gs => gs.Goods).AsNoTracking();
        }
    }
}
