using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class GoodsCreater : ICreater<Goods>
    {
        UnitOfWork UoF;

        public GoodsCreater(UnitOfWork uoF)
        {
            UoF = uoF;
        }

        public void Creat(Goods entity)
        {
            UoF.Goods.Creat(entity);
            UoF.Save();
        }
    }
}
