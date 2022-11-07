using BLL.Interfaces;
using UoW;
using DAL.Entities;
using BLL.Exceptions;

namespace BLL
{
    public class GoodsController : ICreater<Goods>, IDeleter<Goods>, IUpDater<Goods>, IGiver<Goods>
    {
        private UnitOfWork UoW;

        public GoodsController(UnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Creat(Goods entity)
        {
            UoW.Goods.Creat(entity);
            UoW.Save();
        }

        public void Delete(Goods entity)
        {
            UoW.Goods.Delete(entity.Id);
            UoW.Save();
        }

        public void UpDate(Goods entity)
        {
            UoW.Goods.Update(entity);
            UoW.Save();
        }

        public List<Goods> GetAll()
        {
            return UoW.Goods.GetAll().ToList();
        }

        public Goods GetCurrent(int id)
        {
            var result = UoW.Goods.Get(id);
            if (result != null)
                return result;
            throw new GoodsException("Current goods didn't fint");
        }

        public List<Goods> GetAllFollowing(int standard)
        {
            return UoW.Goods.GetAllInCategory(standard).ToList();
        }

        public List<Goods> GetInStock()
        {
            return UoW.Goods.GetInStock().ToList();
        }
    }
}
