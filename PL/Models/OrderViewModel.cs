using DAL.Statuses;

namespace PL.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public uint Count { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int GoodsId { get; set; }
        public GoodsViewModel Goods { get; set; } = null!;
        public int OrderListId { get; set; }
        public decimal Sum;
    }
}
