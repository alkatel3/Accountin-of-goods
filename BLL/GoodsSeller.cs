using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;
using DAL.EF;

namespace BLL
{
    public class GoodsSeller : ISeller<GoodsBLL>, IDisposable
    {
        EFUnitOfWork UoW;
        GoodsController goodsController;
        public GoodsSeller(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
            goodsController = new GoodsController(UoW);
        }

        public async void Sell(int Count, GoodsBLL goods)
        {
            goods.Count -= Count;
            goodsController.UpDate(goods);
            if (goods.Count < 0)
            {
                var uow = new EFUnitOfWork(new ApplicationContext());
                GoodsProvider provider= new GoodsProvider(uow);
                await Task.Run(()=> provider.Deliver(Count*2,goods));
            }
        }
        
        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}
