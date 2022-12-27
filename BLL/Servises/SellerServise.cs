using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using DAL.Priority;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BLL.Servises
{
    public class SellerServise : ISellerServise
    {
        private readonly IUnitOfWork UoW;
        public event EventHandler<int> EntityDidntFind = null!;
        public event EventHandler<PropertyValues> CantSaveChanges = null!;

        public SellerServise(IUnitOfWork UoW)
        {
            this.UoW = UoW;
            UoW.CantSaveChanges += CantSaveChange;
        }

        public void AddGoods(GoodsDTO goods, uint count)
        {
            if (goods == null)
            {
                OnEvent(EntityDidntFind, 0);
                return;
            }
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            }).CreateMapper();
            var goodsDAL = mapper.Map<GoodsDTO, Goods>(goods);

            if (UoW.Goods.Get(goodsDAL.Id) == null)
            {
                if (goodsDAL.GoodsInStock == null)
                    goodsDAL.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;

                UoW.Goods.Creat(goodsDAL);
                UoW.Save();
            }

            if (count == 0)
            {
                return;
            }
            if (goodsDAL.GoodsStatus == DAL.Statuses.GoodsStatus.NotAvailable)
            {
                var goodsInStock = new GoodsInStock()
                {
                    GoodsId = goodsDAL.Id,
                    Count = count
                };
                UoW.GoodsInStock.Creat(goodsInStock);
                goodsDAL.GoodsStatus = DAL.Statuses.GoodsStatus.InStock;
                UoW.Save();
            }
            else
            {
                goodsDAL.GoodsInStock.Count += count;
            }
            UoW.Goods.Update(goodsDAL);
            UoW.Save();
        }

        public void CreateGoods(GoodsDTO goods)
        {
            goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GoodsDTO, Goods>()).CreateMapper();

            var goodsDAL = mapper.Map<GoodsDTO, Goods>(goods);

            UoW.Goods.Creat(goodsDAL);

            UoW.Save();
        }

        public GoodsDTO? GetCurrentGoods(int id)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Goods, GoodsDTO>()).CreateMapper();
            var goodsDAL = UoW.Goods.Get(id);
            var GoodsDTO = mapper.Map<Goods, GoodsDTO>(goodsDAL);
            if (GoodsDTO == null)
            {
                OnEvent(EntityDidntFind, id);
            }
            return GoodsDTO;
        }

        public List<GoodsDTO> GetAllGoods()
        {
            var GoodsDAL = UoW.Goods.GetAll();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
                cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            }).CreateMapper();
            return mapper.Map<IEnumerable<Goods>, List<GoodsDTO>>(GoodsDAL);
        }

        public List<GoodsInStockDTO> GetGoodsInStocks()
        {
            var GoodsInStockDAL = UoW.GoodsInStock.GetAll();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Goods, GoodsDTO>();
                cfg.CreateMap<GoodsInStock, GoodsInStockDTO>().ForMember("Goods", o => o.MapFrom(gs => gs.Goods));
            }).CreateMapper();
            return mapper.Map<IEnumerable<GoodsInStock>, List<GoodsInStockDTO>>(GoodsInStockDAL);
        }

        public List<OrderListDTO> GetOrderLists()
        {
            var orderListDAL = UoW.OrderLists.GetAll();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderList, OrderListDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<OrderList>, List<OrderListDTO>>(orderListDAL);
        }

        public List<QueueForPurchaseDTO> GetQueueForPurchase()
        {
            var queueForPurchasesDAL = UoW.QueueForPurchase.GetAll();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
                cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
                cfg.CreateMap<QueueForPurchase, QueueForPurchaseDTO>().ForMember("Goods", o => o.MapFrom(qp => qp.Goods));
            }).CreateMapper();
            return mapper.Map<IEnumerable<QueueForPurchase>, List<QueueForPurchaseDTO>>(queueForPurchasesDAL);
        }

        public OrderListDTO GetCurrentOrderList(int id)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderList, OrderListDTO>()).CreateMapper();
            var orderListDAL = UoW.OrderLists.Get(id);
            var orderListDTO = mapper.Map<OrderList, OrderListDTO>(orderListDAL); ;
            if (orderListDTO == null)
            {
                OnEvent(EntityDidntFind, id);
            }
            return orderListDTO;
        }

        public void ProcessOrder(OrderDTO order)
        {
            //use transaction 
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
                cfg.CreateMap<OrderDTO, Order>().ForMember("Goods", o => o.MapFrom(o => o.Goods));
            }).CreateMapper();
            var orderDAL = mapper.Map<OrderDTO, Order>(order);
            var GoodsInStock = UoW.GoodsInStock.Find(gs => gs.GoodsId == orderDAL.GoodsId).FirstOrDefault();
            if (order.OrderStatus == DAL.Statuses.OrderStatus.AwaitingShipment && GoodsInStock != null)
            {
                GoodsInStock.Count -= order.Count;
                uint needOrder = 0;
                foreach (var item in UoW.Orders.Find(o => o.GoodsId == order.GoodsId && o.OrderStatus == DAL.Statuses.OrderStatus.AwaitingShipment))
                {
                    if (item.Id == order.Id)
                    {
                        continue;
                    }
                    if (GoodsInStock.Count < item.Count)
                    {
                        item.OrderStatus = DAL.Statuses.OrderStatus.WaitingForDelivery;
                        UoW.Orders.Update(item);
                        needOrder += item.Count;
                    }
                }
                if (GoodsInStock.Count == 0)
                {
                    orderDAL.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
                    UoW.Goods.Update(orderDAL.Goods);
                    UoW.GoodsInStock.Delete(GoodsInStock.Id);
                }
                else
                {
                    UoW.GoodsInStock.Update(GoodsInStock);
                }
                UoW.Orders.Delete(orderDAL.Id);
                UoW.Save();
                AddGoodsToQueueForPurchase(orderDAL.Goods, needOrder);
            }
        }

        public UserDTO CreatAccount(UserDTO seller)
        {
            seller.UserStatus = DAL.Statuses.UserStatus.Seller;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            var sellerDAL = mapper.Map<UserDTO, User>(seller);
            UoW.Users.Creat(sellerDAL);

            UoW.Save();
            return GetAccount(seller.PhoneNumber);
        }

        public UserDTO GetAccount(int phoneNumber)
        {
            var clientDAL = UoW.Users.Find(c => c.PhoneNumber == phoneNumber).ToList().FirstOrDefault();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Goods, GoodsDTO>();
                cfg.CreateMap<Order, OrderDTO>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
                cfg.CreateMap<OrderList, OrderListDTO>().ForMember("Orders", ol => ol.MapFrom(o => o.Orders));
                cfg.CreateMap<User, UserDTO>().ForMember("OrderList", c => c.MapFrom(ol => ol.OrderList));
            }).CreateMapper();
            var clientDTO = mapper.Map<User, UserDTO>(clientDAL);
            return clientDTO;
        }

        public List<OrderDTO> GetOrders()
        {
            var ordersDAL = UoW.Orders.GetAll();
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Goods, GoodsDTO>();
                cfg.CreateMap<Order, OrderDTO>().ForMember("Goods", o => o.MapFrom(o => o.Goods));
            }).CreateMapper();
            return mapper.Map<IEnumerable<Order>, List<OrderDTO>>(ordersDAL);
        }

        public void UpdateGoods(GoodsDTO goods)
        {

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            }).CreateMapper();

            var goodsDAL = mapper.Map<GoodsDTO, Goods>(goods);

            UoW.Goods.Update(goodsDAL);

            UoW.Save();
        }

        public void UpdateGoods(GoodsInStockDTO goods)
        {
            if (goods.Count == 0)
            {
                goods.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
            }
            var mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsDTO, Goods>();
            cfg.CreateMap<GoodsInStockDTO, GoodsInStock>().ForMember("Goods", o => o.MapFrom(gs => gs.Goods));
        }).CreateMapper();

            var goodsDAL = mapper.Map<GoodsInStockDTO, GoodsInStock>(goods);

            UoW.GoodsInStock.Update(goodsDAL);

            UoW.Save();
        }

        public void BringGoodsFromQueueForPurchase(QueueForPurchaseDTO queueForPurchase)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
                cfg.CreateMap<QueueForPurchaseDTO, QueueForPurchase>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
            }).CreateMapper();
            var queueForPurchaseDAL = mapper.Map<QueueForPurchaseDTO, QueueForPurchase>(queueForPurchase);

            var GoodsInStock = UoW.GoodsInStock.Find(g => g.GoodsId == queueForPurchase.GoodsId).FirstOrDefault();
            if (GoodsInStock == null)
            {
                GoodsInStock = new GoodsInStock()
                {
                    GoodsId = queueForPurchaseDAL.GoodsId,
                    Count = queueForPurchaseDAL.Count,
                };
                queueForPurchaseDAL.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.InStock;
                UoW.Goods.Update(queueForPurchaseDAL.Goods);
                UoW.GoodsInStock.Creat(GoodsInStock);
            }
            else
            {
                GoodsInStock.Count += queueForPurchaseDAL.Count;
                GoodsInStock.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.InStock;
                UoW.GoodsInStock.Update(GoodsInStock);
            }

            var orders = UoW.Orders.Find(o =>
            o.GoodsId == queueForPurchaseDAL.GoodsId &&
            o.OrderStatus == DAL.Statuses.OrderStatus.WaitingForDelivery);

            foreach (var order in orders)
            {
                order.OrderStatus = DAL.Statuses.OrderStatus.AwaitingShipment;
                UoW.Orders.Update(order);
            }
            UoW.QueueForPurchase.Delete(queueForPurchase.Id);
            UoW.Save();

        }

        private void OnEvent<T>(EventHandler<T>? SomeEvent, T? e)
        {
            SomeEvent?.Invoke(this, e);
        }

        private void CantSaveChange(object sender, PropertyValues e)
        {
            CantSaveChanges.Invoke(sender, e);
        }

        private void AddGoodsToQueueForPurchase(Goods goods, uint count)
        {
            if (count == 0)
            {
                return;
            }
            if (goods.GoodsStatus == DAL.Statuses.GoodsStatus.Waiting)
            {
                if (goods.QueueForPurchase == null)
                {
                    //goods = UoW.Goods.GetAll().First(g => g.Id == goods.Id);
                    goods.QueueForPurchase = UoW.QueueForPurchase.Find(q => q.GoodsId == goods.Id).FirstOrDefault();
                }
                goods.QueueForPurchase.Count += count;
                UoW.QueueForPurchase.Update(goods.QueueForPurchase);
                return;
            }
            var QueueForPurchase = new QueueForPurchase()
            {
                GoodsId = goods.Id,
                Count = count,
                Priority = DAL.Priority.GoodsInQueuePriority.Medium
            };
            UoW.QueueForPurchase.Creat(QueueForPurchase);
            goods.GoodsStatus = DAL.Statuses.GoodsStatus.Waiting;
            UoW.Goods.Update(goods);
            UoW.Save();
        }
    }
}
