using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;

namespace BLL
{
    public class GoodsProvider:IProvider<GoodsBLL>
    {
        public void Deliver(int Count, GoodsBLL goods)
        {
            //Thread.Sleep(30000);
            goods.Count += Count;
        }
    }
}
