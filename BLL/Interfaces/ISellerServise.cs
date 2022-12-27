using BLL.DTO;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BLL.Interfaces
{
    public interface ISellerServise : IUserServise
    {
        public event EventHandler<int> EntityDidntFind;
        public event EventHandler<PropertyValues> CantSaveChanges;

        List<GoodsDTO> GetAllGoods();
        List<GoodsInStockDTO> GetGoodsInStocks();
        List<OrderListDTO> GetOrderLists();
        List<QueueForPurchaseDTO> GetQueueForPurchase();
        List<OrderDTO> GetOrders();
        GoodsDTO GetCurrentGoods(int id);
        OrderListDTO GetCurrentOrderList(int id);

        void UpdateGoods(GoodsDTO goods);
        void UpdateGoods(GoodsInStockDTO goods);

        void CreateGoods(GoodsDTO goods);
        void AddGoods(GoodsDTO goods, uint count);

        void ProcessOrder(OrderDTO order);

        void BringGoodsFromQueueForPurchase(QueueForPurchaseDTO queueForPurchase);
    }
}
