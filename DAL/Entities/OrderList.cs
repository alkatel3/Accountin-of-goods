namespace DAL.Entities
{
    public class OrderList:BaseEntity<int>
    {
        public ICollection<Order> Orders { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public OrderList()
        {
            Orders = new List<Order>();
        }
    }
}
