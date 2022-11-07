using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class GoodsDeleter:IDeleter<Goods>
    {
        private UnitOfWork UoW;

        public GoodsDeleter(UnitOfWork uoW)
        {
            UoW = uoW;
        }

        public void Delete(Goods entity)
        {
            UoW.Goods.Delete(entity.Id);
            UoW.Save();
        }
    }
}
