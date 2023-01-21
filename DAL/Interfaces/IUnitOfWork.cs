using DAL.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Interfaces
{
    public interface IUnitOfWork
    {
        event EventHandler<PropertyValues> CantSaveChanges;

        IRepository<int,Goods> Goods { get; }
        IRepository<int,GoodsInStock> GoodsInStock { get; }
        IRepository<int,Order > Orders { get; }
        IRepository<int, OrderList> OrderLists { get; }
        IRepository<int,QueueForPurchase> QueueForPurchase { get; }
        IRepository<int, User> Users { get; }

        //void CreateSavepoint(string savepointName);

        //void TransactionCommit();

        //void RollbackSavepoint(string savepointName);

        void Save();
    }
}
