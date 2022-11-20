﻿using BLL.Interfaces;
using DAL.Repositories;
using BLL.Entities;

namespace BLL
{
    public class GoodsProvider:IProvider<GoodsBLL>, IDisposable
    {
        EFUnitOfWork UoW;
        GoodsController goodsController;
        public GoodsProvider(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
            goodsController = new GoodsController(UoW);
        }

        public void Deliver(int Count, GoodsBLL goods)
        {
            Thread.Sleep(30000);
            goods.Count += Count;
            goodsController.UpDate(goods);
        }

        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}
