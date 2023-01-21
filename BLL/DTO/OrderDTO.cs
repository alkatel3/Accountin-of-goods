using DAL.Statuses;

namespace BLL.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public uint Count { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int GoodsId { get; set; }
        public GoodsDTO Goods { get; set; } = null!;
        public int OrderListId { get; set; }
        public OrderListDTO OrderList { get; set; } = null!;
        public decimal Sum => Goods.Price * Count;
        public byte[] Version { get; set; } = null!;
    }
}
