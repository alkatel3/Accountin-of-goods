using BLL.DTO;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BLL.Servises
{
    public class SellerServise : ISellerServise
    {
        private readonly IUnitOfWork UoW;
        public event EventHandler<int> EntityDidntFind = null!;
        public event EventHandler<PropertyValues> CantSaveChanges = null!;
        public event EventHandler<UserDTO> ClientNull = null!;

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

            var goodsDAL = Mappers.GoodsDtoGoodsMapper.Map<GoodsDTO, Goods>(goods);

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
            if (goods.Name == null)
            {
                return;
            }
            if (goods.GoodsInStock == null||goods.GoodsInStock.Count<=0)
            {
                goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
            }
            else
            {
                goods.GoodsStatus = DAL.Statuses.GoodsStatus.InStock;
            }

            var goodsDAL = Mappers.GoodsDtoGoodsMapper.Map<GoodsDTO, Goods>(goods);

            UoW.Goods.Creat(goodsDAL);

            UoW.Save();
        }

        public GoodsDTO? GetCurrentGoods(int id)
        {
            var goodsDAL = UoW.Goods.Get(id);
            var GoodsDTO = Mappers.GoodsGoodsDtoMapper.Map<Goods, GoodsDTO>(goodsDAL);
            if (GoodsDTO == null)
            {
                OnEvent(EntityDidntFind, id);
            }
            return GoodsDTO;
        }

        public void DeleteGoods(int id)
        {
            UoW.Goods.Delete(id);
            UoW.Save();
        }

        public List<GoodsDTO> GetAllGoods()
        {
            var GoodsDAL = UoW.Goods.GetAll();
            return Mappers.GoodsGoodsDtoMapper.Map<IEnumerable<Goods>, List<GoodsDTO>>(GoodsDAL);
        }

        public List<GoodsInStockDTO> GetGoodsInStocks()
        {
            var GoodsInStockDAL = UoW.GoodsInStock.GetAll();
            return Mappers.GoodsInStockGoodsInStockDtoMapper
                .Map<IEnumerable<GoodsInStock>, List<GoodsInStockDTO>>(GoodsInStockDAL);
        }

        public List<OrderListDTO> GetOrderLists()
        {
            var orderListDAL = UoW.OrderLists.GetAll();
            return Mappers.OrderListOrderListDTOMapper
                .Map<IEnumerable<OrderList>, List<OrderListDTO>>(orderListDAL);
        }

        public List<QueueForPurchaseDTO> GetQueueForPurchase()
        {
            var queueForPurchasesDAL = UoW.QueueForPurchase.GetAll();
            return Mappers.QueueForPurchaseQueueForPurchaseDtoMapper
                .Map<IEnumerable<QueueForPurchase>, List<QueueForPurchaseDTO>>(queueForPurchasesDAL);
        }

        public OrderListDTO GetCurrentOrderList(int id)
        {
            var orderListDAL = UoW.OrderLists.Get(id);
            var orderListDTO = Mappers.OrderListOrderListDTOMapper
                .Map<OrderList, OrderListDTO>(orderListDAL);
            if (orderListDTO == null)
            {
                OnEvent(EntityDidntFind, id);
            }
            return orderListDTO;
        }

        public OrderDTO GetCurrentOrder(int id)
        {
            var orderDAL = UoW.Orders.Get(id);
            var orderDTO = Mappers.OrderOrderDtoMapper.Map<Order, OrderDTO>(orderDAL);
            if (orderDTO == null)
            {
                OnEvent(EntityDidntFind, id);
            }

            return orderDTO;
        }

        public void ProcessOrder(OrderDTO order)
        {
            //use transaction 
            //UoW.CreateSavepoint("BeforeProcessOrder");
            var orderDAL = Mappers.OrderDtoOrderMapper
                .Map<OrderDTO, Order>(order);
            if (order.OrderStatus == DAL.Statuses.OrderStatus.AwaitingShipment && orderDAL.Goods.GoodsInStock != null)
            {
                orderDAL.Goods.GoodsInStock.Count -= order.Count;

                var needOrder = SetOrdersAsWaitingForDelivery(orderDAL);

                if (orderDAL.Goods.GoodsInStock.Count == 0)
                {
                    orderDAL.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
                    UoW.Goods.Update(orderDAL.Goods);
                    UoW.GoodsInStock.Delete(orderDAL.Goods.GoodsInStock.Id);
                }
                else
                {
                    UoW.GoodsInStock.Update(orderDAL.Goods.GoodsInStock);
                }
                UoW.Orders.Delete(orderDAL.Id);
                UoW.Save();

                AddGoodsToQueueForPurchase(orderDAL.GoodsId, needOrder);
                //UoW.TransactionCommit();
            }
        }

        public QueueForPurchaseDTO GetCurrentInQueueForPurchaseDTO(int id)
        {
            var currentDal = UoW.QueueForPurchase.Get(id);
            var currentDto = Mappers.QueueForPurchaseQueueForPurchaseDtoMapper
                .Map<QueueForPurchase, QueueForPurchaseDTO>(currentDal);
            if (currentDto == null)
            {
                OnEvent(EntityDidntFind, id);
            }

            return currentDto;
        }

        public UserDTO CreatAccount(UserDTO seller)
        {
            if (seller.FirstName == null || seller.LastName == null || seller.PhoneNumber <= 0)
            {
                OnEvent(ClientNull, seller);
                return null;
            }
            seller.UserStatus = DAL.Statuses.UserStatus.Seller;
            var sellerDAL = Mappers.UserDTOUsermapper
                .Map<UserDTO, User>(seller);
            var TryGet = UoW.Users.Find(u => u.PhoneNumber == seller.PhoneNumber).FirstOrDefault();
            if (TryGet == null)
            {
                UoW.Users.Creat(sellerDAL);
                UoW.Save();
                return GetAccount(sellerDAL.PhoneNumber);
            }
            else
            {
                TryGet.FirstName = sellerDAL.FirstName;
                TryGet.LastName = sellerDAL.LastName;
                TryGet.UserStatus = sellerDAL.UserStatus;
                UoW.Users.Update(TryGet);
                UoW.Save();
                return Mappers.UserUserDTOmapper.Map<User, UserDTO>(TryGet);
            }
        }

        public UserDTO GetAccount(int phoneNumber)
        {
            var clientDAL = UoW.Users.Find(c => c.PhoneNumber == phoneNumber).ToList().FirstOrDefault();
            var clientDTO = Mappers.UserUserDTOmapper.Map<User, UserDTO>(clientDAL);
            return clientDTO;
        }

        public List<OrderDTO> GetOrders()
        {
            var ordersDAL = UoW.Orders.GetAll();
            return Mappers.OrderOrderDtoMapper.Map<IEnumerable<Order>, List<OrderDTO>>(ordersDAL);
        }

        public void UpdateGoods(GoodsDTO goods)
        {
            if (goods.Name == null || goods.Price<=0 )
            {
                return;
            }

            var goodsDAL = Mappers.GoodsDtoGoodsMapper.Map<GoodsDTO, Goods>(goods);

            if (goodsDAL.GoodsInStock.Count > 0)
            {
                goodsDAL.GoodsStatus = DAL.Statuses.GoodsStatus.InStock;
            }
            else if(goodsDAL.GoodsInStock.Count == 0)
            {
                goodsDAL.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
            }

            UoW.Goods.Update(goodsDAL);

            UoW.Save();
        }

        public void UpdateGoods(GoodsInStockDTO goods)
        {
            if (goods.Count == 0)
            {
                goods.Goods.GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable;
            }

            var goodsDAL = Mappers.GoodsInStockDtoGoodsInStockMapper.Map<GoodsInStockDTO, GoodsInStock>(goods);
            UoW.GoodsInStock.Update(goodsDAL);
            UoW.Save();
        }

        public void BringGoodsFromQueueForPurchase(QueueForPurchaseDTO queueForPurchase)
        {
            var queueForPurchaseDAL = Mappers.QueueForPurchaseDtoQueueForPurchaseMapper
                .Map<QueueForPurchaseDTO, QueueForPurchase>(queueForPurchase);

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
            o.OrderStatus == DAL.Statuses.OrderStatus.WaitingForDelivery &&
            o.Count<=queueForPurchaseDAL.Count).ToList();

            foreach (var order in orders)
            {
                order.OrderStatus = DAL.Statuses.OrderStatus.AwaitingShipment;
            }
            UoW.Orders.UpdateRange(orders);
            UoW.QueueForPurchase.Delete(queueForPurchase.Id);
            UoW.Save();
        }

        private uint SetOrdersAsWaitingForDelivery(Order order)
        {
            uint needOrder = 0;
            var Orders = UoW.Orders.Find(o =>
                o.GoodsId == order.GoodsId &&
                o.OrderStatus == DAL.Statuses.OrderStatus.AwaitingShipment &&
                o.Count > order.Goods.GoodsInStock.Count &&
                o.Id != order.Id).ToList();
            foreach (var item in Orders)
            {
                item.OrderStatus = DAL.Statuses.OrderStatus.WaitingForDelivery;
                needOrder += item.Count;
            }
            UoW.Orders.UpdateRange(Orders);
            UoW.Save();
            return needOrder;
        }

        private void OnEvent<T>(EventHandler<T>? SomeEvent, T? e)
        {
            SomeEvent?.Invoke(this, e);
        }

        private void CantSaveChange(object sender, PropertyValues e)
        {
            CantSaveChanges?.Invoke(sender, e);
        }

        private void AddGoodsToQueueForPurchase(int goodsId, uint count)
        {
            if (count == 0)
            {
                return;
            }

            var goods = UoW.Goods.Get(goodsId);
            if (goods.GoodsStatus == DAL.Statuses.GoodsStatus.Waiting)
            {
                goods.QueueForPurchase.Count += count;
                UoW.QueueForPurchase.Update(goods.QueueForPurchase);
                UoW.Save();
                return;
            }
            var QueueForPurchase = new QueueForPurchase()
            {
                GoodsId = goods.Id,
                Count = count,
                Priority = DAL.Priority.GoodsInQueuePriority.Medium
            };
            //UoW.QueueForPurchase.Creat(QueueForPurchase);
            goods.QueueForPurchase = QueueForPurchase;
            goods.GoodsStatus = DAL.Statuses.GoodsStatus.Waiting;
            UoW.Goods.Update(goods);
            UoW.Save();
        }
    }
}
