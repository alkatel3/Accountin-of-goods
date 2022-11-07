using BLL.Interfaces;
using UoW;

namespace BLL
{
    public class GoodsSeller : ISeller
    {
        UnitOfWork UoW=new UnitOfWork();
        GoodsProvider provider;
        public GoodsSeller(UnitOfWork UoW)
        {
            this.UoW = UoW;
            provider = new GoodsProvider(UoW);
        }

        public async void Sell(int Count, int id)
        {
            var goods =new GoodsController(UoW);
            var current = goods.GetCurrent(id);
            current.Count -= Count;
            UoW.Goods.Update(current);
            UoW.Save();
            if(current.Count < 0)
            {
                await Task.Run(()=> provider.Deliver(Count*2,id));
            }
        }
    }
}
