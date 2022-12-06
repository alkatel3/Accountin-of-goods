using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private ApplicationContext db;
        private GoodsRepository? goodsRepository;
        private CategoryRepository? categoryRepository;

        public EFUnitOfWork(ApplicationContext db)
        {
            this.db = db;
        }

        public IRepository<Category> Categories
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);

                return categoryRepository;
            }
        }

        public IRepository<Goods> Goods
        {
            get
            {
                if (goodsRepository == null)
                    goodsRepository = new GoodsRepository(db);

                return goodsRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
            db.ChangeTracker?.Clear();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
