using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;

namespace BLL
{
    public class GoodsSeller : ISeller<GoodsBLL>
    {
        EFUnitOfWork UoW;
        GoodsProvider provider;
        public GoodsSeller(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
            provider = new GoodsProvider(UoW);
        }

        public async void Sell(int Count, GoodsBLL goods)
        {
            var goodsController =new GoodsController(UoW);
            var current = goodsController.GetCurrent(goods.Id);
            current.Count -= Count;
            //UoW.Goods.Update(goods);
            goodsController.UpDate(goods);
            if (current.Count < 0)
            {
                await Task.Run(()=> provider.Deliver(Count*2,goods));
            }
        }
    }
}
