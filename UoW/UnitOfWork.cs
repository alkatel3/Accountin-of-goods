using DAL;
using UoW.Repositories;

namespace UoW
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationContext db;
        private GoodsRepository? goodsRepository;
        private CategoryRepository? categoryRepository;

        public UnitOfWork(string connectionString= "Data Source=Goods.db")
        {
            db = new(connectionString);
        }

        public GoodsRepository Goods
        {
            get
            {
                if (goodsRepository == null)
                    goodsRepository = new GoodsRepository(db);

                return goodsRepository;
            }
        }

        public CategoryRepository Categories
        {
            get
            {
                if (categoryRepository == null)
                    categoryRepository = new CategoryRepository(db);

                return categoryRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
