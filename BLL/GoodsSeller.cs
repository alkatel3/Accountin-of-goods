using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;
using DAL.EF;

namespace BLL
{
    public class GoodsSeller : ISeller<GoodsBLL>
    {
        GoodsController goodsController;
        public GoodsSeller(GoodsController controller)
        {
            goodsController = controller;
        }

        public /*async*/ void Sell(int Count, GoodsBLL goods)
        {
            goods.Count -= Count;
            if (goods.Count <= 0)
            {
                GoodsProvider provider= new GoodsProvider();
                /* await Task.Run(()=> */
                provider.Deliver(Count*2,goods);
            }
            goodsController.UpDate(goods);
        }
    }
}
