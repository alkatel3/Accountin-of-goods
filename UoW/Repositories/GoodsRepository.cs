﻿using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace UoW.Repositories
{
    public class GoodsRepository : IRepository<Goods>
    {
        private ApplicationContext db;

        public GoodsRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Goods> GetAll()
        {
            return db.Goods.Include(g=>g.Category);
        }

        public IEnumerable<Goods> GetAllInCategory(int CategoryId)
        {
            return db.Goods.Where(goods => goods.CategoryId.Equals(CategoryId)).Include(g => g.Category);
        }

        public IEnumerable<Goods> GetInStock()
        {
            return db.Goods.Where(goods => goods.Count > 0).Include(g => g.Category);
        }

        public Goods? Get(int id)
        {
            return db.Goods.Find(id);
        }

        public void Creat(Goods goods)
        {
            db.Goods.Add(goods);
        }

        public void Update(Goods goods)
        {
            db.Entry(goods).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Goods? goods = db.Goods.Find(id);
            if (goods != null)
                db.Goods.Remove(goods);
        }
    }
}