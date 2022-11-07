using UoW;
using DAL.Entities;
using BLL.Exceptions;
using BLL.Interfaces;

namespace BLL
{
    public class GetGoods:IShow<Goods>
    {
        private UnitOfWork UoW;

        public GetGoods(UnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public List<Goods> GetAll()
        {
            return UoW.Goods.GetAll().ToList();
        }

        public Goods GetCurrent(int id)
        {
            var result=UoW.Goods.Get(id);
            if (result != null)
                return result;
            throw new GoodsException("Current goods didn't fint");
        }

        public List<Goods> GetAllFollowing(int standard)
        {
            return UoW.Goods.GetAllInCategory(standard).ToList();
        }

        public  List<Goods> GetInStock()
        {
            return UoW.Goods.GetInStock().ToList();
        }
    }
}
