using BLL.Interfaces;
using UoW;

namespace BLL
{
    public class Provider:IProvider
    {
        UnitOfWork UoW;
        public Provider(UnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Deliver(int Count, int id)
        {
            Thread.Sleep(60000);
            var goods = new GetGoods(UoW);
            var current = goods.GetCurrent(id);
            current.Count += Count;
            UoW.Goods.Update(current);
            UoW.Save();
        }
    }
}
