using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;

namespace BLL
{
    public class GoodsProvider:IProvider<GoodsBLL>
    {
        EFUnitOfWork UoW;
        public GoodsProvider(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Deliver(int Count, GoodsBLL goods)
        {
            Thread.Sleep(60000);
            var goodsController = new GoodsController(UoW);
            var current = goodsController.GetCurrent(goods.Id);
            current.Count += Count;
            goodsController.UpDate(goods);
        }
    }
}
