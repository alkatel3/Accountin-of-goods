using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class GoodsUpDater : IUpDater<Goods>
    {
        private UnitOfWork UoW;

        public GoodsUpDater(UnitOfWork uoW)
        {
            UoW = uoW;
        }

        public void UpDate(Goods entity)
        {
            UoW.Goods.Update(entity);
            UoW.Save();
        }
    }
}
