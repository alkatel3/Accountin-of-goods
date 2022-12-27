using DAL.Statuses;

namespace DAL.Entities
{
    public class Order:BaseEntity<int>
    {
        public uint Count { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int GoodsId { get; set; }
        public Goods Goods { get; set; }
        public int OrderListId { get; set; }
        public OrderList OrderList { get; set; }
    }
}
