using BLL.DTO;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BLL.Interfaces
{
    public interface ICustomerService : IUserServise
    {
        public event EventHandler<GoodsDTO> GoodsNull;
        public event EventHandler<UserDTO> ClientNull;
        public event EventHandler<PropertyValues> CantSaveChanges;

        List<GoodsDTO> GetAllGoods();
        List<GoodsInStockDTO> GetGoodsInStock();
        GoodsDTO GetCurrentGoods(int id);
        void CreatOrder(UserDTO client, GoodsDTO goods, uint count);
        OrderListDTO GetOrdetList(UserDTO clientDTO);
    }
}
