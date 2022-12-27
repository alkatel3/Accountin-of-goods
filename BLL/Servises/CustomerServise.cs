using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BLL.Servises
{
    public class CustomerServise : ICustomerService
    {
        private readonly IUnitOfWork UoW;
        public event EventHandler<PropertyValues> CantSaveChanges=null!;
        public event EventHandler<GoodsDTO> GoodsNull = null!;
        public event EventHandler<UserDTO> ClientNull = null!;
        public CustomerServise(IUnitOfWork UoW)
        {
            this.UoW = UoW;
            this.UoW.CantSaveChanges += CantSaveChange;
        }

        public UserDTO CreatAccount(UserDTO client)
        {
            client.UserStatus = DAL.Statuses.UserStatus.Customer;
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>()).CreateMapper();
            var clientDAL = mapper.Map<UserDTO, User>(client);
            UoW.Users.Creat(clientDAL);
            UoW.Save();
            return GetAccount(client.PhoneNumber);
        }

        public void CreatOrder(UserDTO client, GoodsDTO goods, uint count)
        {
            var goodsMapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<QueueForPurchaseDTO, QueueForPurchase>();
                cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("QueueForPurchase", o => o.MapFrom(g => g.QueueForPurchase));
                cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            }).CreateMapper();
            var GoodsDAL = goodsMapper.Map<GoodsDTO, Goods>(goods);

            var clientMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GoodsDTO, Goods>();
                cfg.CreateMap<OrderDTO, Order>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
                cfg.CreateMap<OrderListDTO, OrderList>().ForMember("Orders", ol => ol.MapFrom(o => o.Orders));
                cfg.CreateMap<UserDTO, User>().ForMember("OrderList", o => o.MapFrom(u => u.OrderList));
                }).CreateMapper();
            var clientDAL = clientMapper.Map<UserDTO, User>(client);

            if (goods == null)
            {
                OnEvent(GoodsNull, goods);
                return;
            }
            if (client == null)
            {
                OnEvent(ClientNull, client);
                return;
            }

            if (client.OrderList == null)
            {
                var OrderList = new OrderList();
                clientDAL.OrderList = OrderList;
                UoW.Users.Update(clientDAL);
                UoW.Save();
                clientDAL = UoW.Users.Get(clientDAL.Id);
            }

            Order order = new()
            {
                GoodsId = GoodsDAL.Id,
                Count = count,
                OrderListId = clientDAL.OrderList.Id,
                OrderStatus = DAL.Statuses.OrderStatus.AwaitingShipment
            };
            UoW.Orders.Creat(order);

            if (GoodsDAL.GoodsStatus == DAL.Statuses.GoodsStatus.NotAvailable || GoodsDAL.GoodsInStock?.Count<=count)
            {
                AddGoodsToQueueForPurchase(GoodsDAL, count);
                order.OrderStatus = DAL.Statuses.OrderStatus.WaitingForDelivery;
            }

            UoW.Save();
        }

        public UserDTO GetAccount(int phoneNumber)
        {
            var clientDAL = UoW.Users/*.Get(phoneNumber);*/.Find(c=>c.PhoneNumber==phoneNumber).ToList().FirstOrDefault();
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Goods, GoodsDTO>();
                cfg.CreateMap<Order, OrderDTO>().ForMember("Goods",o=>o.MapFrom(g=>g.Goods));
                cfg.CreateMap<OrderList, OrderListDTO>().ForMember("Orders", ol => ol.MapFrom(o => o.Orders));
                cfg.CreateMap<User, UserDTO>().ForMember("OrderList", c => c.MapFrom(ol => ol.OrderList));
            }).CreateMapper();
            var clientDTO = mapper.Map<User, UserDTO>(clientDAL);
            return clientDTO;
        }
        
        public List<GoodsDTO> GetAllGoods()
        {
            var GoodsDAL = UoW.Goods.GetAll();

            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<QueueForPurchase, QueueForPurchaseDTO>();
                cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
                cfg.CreateMap<Goods, GoodsDTO>().ForMember("QueueForPurchase", o => o.MapFrom(g => g.QueueForPurchase));
                cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            }).CreateMapper();

            return mapper.Map<IEnumerable<Goods>, List<GoodsDTO>>(GoodsDAL);
        }

        public GoodsDTO GetCurrentGoods(int id)
        {
            var GoodsDAL = UoW.Goods.Get(id);

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Goods, GoodsDTO>()).CreateMapper();

            var GoodsDTO = mapper.Map<Goods, GoodsDTO>(GoodsDAL);
            if (GoodsDAL == null)
            {
                OnEvent(GoodsNull, GoodsDTO);
            }

            return GoodsDTO;
        }

        public List<GoodsInStockDTO> GetGoodsInStock()
        {
            var GoodsInStockDAL = UoW.GoodsInStock.GetAll();

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<GoodsInStock, GoodsInStockDTO>()).CreateMapper();

            return mapper.Map<IEnumerable<GoodsInStock>, List<GoodsInStockDTO>>(GoodsInStockDAL);
        }    

        public OrderListDTO GetOrdetList(UserDTO clientDTO)
        {
           return clientDTO.OrderList;
        }

        private void AddGoodsToQueueForPurchase(Goods goods, uint count)
        {
            if (goods.GoodsStatus == DAL.Statuses.GoodsStatus.Waiting)
            {
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
        }

        private void OnEvent<T>(EventHandler<T>? SomeEvent, T? e)
        {
            SomeEvent?.Invoke(this, e);
        }

        private void CantSaveChange(object sender, PropertyValues e)
        {
            CantSaveChanges?.Invoke(sender, e);
        }
    }
}
