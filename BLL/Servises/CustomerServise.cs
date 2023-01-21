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

        public UserDTO? CreatAccount(UserDTO clientDTO)
        {
            if (clientDTO.FirstName == null || clientDTO.LastName == null || clientDTO.PhoneNumber <= 0)
            {
                OnEvent(ClientNull, clientDTO);
                return null;
            }
            clientDTO.UserStatus = DAL.Statuses.UserStatus.Customer;
            var client = Mappers.UserDTOUsermapper.Map<UserDTO, User>(clientDTO);
            var TryGet = UoW.Users.Find(u => u.PhoneNumber == clientDTO.PhoneNumber).FirstOrDefault();
            if (TryGet == null)
            {
                UoW.Users.Creat(client);
                UoW.Save();
                return GetAccount(client.PhoneNumber);
            }
            else
            {
                TryGet.FirstName = client.FirstName;
                TryGet.LastName = client.LastName;
                TryGet.UserStatus = client.UserStatus;
                UoW.Users.Update(TryGet);
                UoW.Save();
                return Mappers.UserUserDTOmapper.Map<User, UserDTO>(TryGet);
            }
        }

        public void CreatOrder(UserDTO client, GoodsDTO goods, uint count)
        {
            var GoodsDAL = Mappers.GoodsDtoGoodsMapper.Map<GoodsDTO, Goods>(goods);

            var clientDAL = Mappers.UserDTOUsermapper.Map<UserDTO, User>(client);

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
            var clientDAL = UoW.Users.Find(c=>c.PhoneNumber==phoneNumber).ToList().FirstOrDefault();

            var clientDTO = Mappers.UserUserDTOmapper.Map<User, UserDTO>(clientDAL);
            return clientDTO;
        }
        
        public List<GoodsDTO> GetAllGoods()
        {
            var GoodsDAL = UoW.Goods.GetAll();
            return Mappers.GoodsGoodsDtoMapper.Map<IEnumerable<Goods>, List<GoodsDTO>>(GoodsDAL);
        }

        public GoodsDTO GetCurrentGoods(int id)
        {
            var GoodsDAL = UoW.Goods.Get(id);
            var GoodsDTO = Mappers.GoodsGoodsDtoMapper.Map<Goods, GoodsDTO>(GoodsDAL);
            if (GoodsDAL == null)
            {
                OnEvent(GoodsNull, GoodsDTO);
            }

            return GoodsDTO;
        }

        public List<GoodsInStockDTO> GetGoodsInStock()
        {
            var GoodsInStockDAL = UoW.GoodsInStock.GetAll();
            return Mappers.GoodsInStockGoodsInStockDtoMapper
                .Map<IEnumerable<GoodsInStock>, List<GoodsInStockDTO>>(GoodsInStockDAL);
        }    

        public OrderListDTO GetOrdetList(UserDTO clientDTO)
        {
            var user = UoW.Users.Find(c=>c.PhoneNumber==clientDTO.PhoneNumber).FirstOrDefault();
            var UserDTO = Mappers.UserUserDTOmapper.Map<User, UserDTO>(user);
            var OrderList = UserDTO?.OrderList;
            return OrderList;
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
