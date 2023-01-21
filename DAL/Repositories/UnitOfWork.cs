using DAL.Interfaces;
using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public event EventHandler<PropertyValues> CantSaveChanges=null!;

        private readonly ApplicationContext db;
        private IRepository<int,Goods>? goods;

        private IRepository<int,GoodsInStock>? goodsInStock;

        private IRepository<int, Order>? orders;

        private IRepository<int, OrderList>? orderLists;

        private IRepository<int, QueueForPurchase>? queuesForPurchase;

        private IRepository<int, User>? users;

       // private IDbContextTransaction transaction;


        public UnitOfWork(ApplicationContext db)
        {
            this.db = db;
           // transaction = db.Database.BeginTransaction();
        }

        public IRepository<int, Goods> Goods
        {
            get
            {
                if (goods == null)
                    goods = new GoodsRepository(db);

                return goods;
            }
        }

        public IRepository<int, GoodsInStock> GoodsInStock
        {
            get
            {
                if (goodsInStock == null)
                    goodsInStock = new GoodsInStockRepository(db);

                return goodsInStock;
            }
        }

        public IRepository<int, Order> Orders
        {
            get
            {
                if (orders == null)
                    orders = new OrderRepository(db);

                return orders;
            }
        }

        public IRepository<int, OrderList> OrderLists
        {
            get
            {
                if (orderLists == null)
                    orderLists = new OrderListRepository(db);

                return orderLists;
            }
        }

        public IRepository<int, QueueForPurchase> QueueForPurchase
        {
            get
            {
                if (queuesForPurchase == null)
                    queuesForPurchase = new QueueForPurchaseRepository(db);

                return queuesForPurchase;
            }
        }

        public IRepository<int, User> Users
        {
            get
            {
                if (users == null)
                    users = new UserRepository(db);

                return users;
            }
        }

        public void Save()
        {
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    if (entry.Entity is Goods ||
                        entry.Entity is GoodsInStock ||
                        entry.Entity is Order ||
                        entry.Entity is OrderList ||
                        entry.Entity is User ||
                        entry.Entity is QueueForPurchase)
                    {
                        var proposedValues = entry.CurrentValues;
                        CantSaveChanges?.Invoke(this, proposedValues);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);
                    }
                }
            }
            finally
            {
                db.ChangeTracker?.Clear();
            }
        }

        //public void CreateSavepoint(string savepointName)
        //{
        //    transaction.CreateSavepoint(savepointName);
        //}

        //public void TransactionCommit()
        //{
        //    transaction.Commit();
        //}

        //public void RollbackSavepoint(string savepointName)
        //{
        //    transaction.RollbackToSavepoint(savepointName);
        //}
    }
}
