using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IRepository<Category> Categories { get; }
        IRepository<Goods> Goods { get; }
        void Save();
    }
}
