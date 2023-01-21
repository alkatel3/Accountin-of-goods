using DAL.Statuses;

namespace DAL.Entities
{
    public class Goods:BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public GoodsStatus GoodsStatus { get; set; }
        public ICollection<Order> Orders { get; set; }
        public QueueForPurchase QueueForPurchase { get; set; }
        public GoodsInStock GoodsInStock { get; set; }

        public Goods()
        {
            Orders = new List<Order>();
        }
    }
}
