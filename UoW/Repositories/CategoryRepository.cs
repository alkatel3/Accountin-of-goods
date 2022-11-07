﻿using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace UoW.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private ApplicationContext db;

        public CategoryRepository(ApplicationContext context)
        {
            this.db = context;
        }

        public IEnumerable<Category> GetAll()
        {
            return db.Categories;
        }

        public Category? Get(int id)
        {
            return db.Categories.Find(id);
        }

        public void Creat(Category category)
        {
            db.Categories.Add(category);
        }

        public void Update(Category category)
        {
            db.Entry(category).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            Category? Category = db.Categories.Find(id);
            if (Category != null)
                db.Categories.Remove(Category);
        }
    }
}