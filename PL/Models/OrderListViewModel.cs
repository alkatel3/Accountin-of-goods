namespace PL.Models
{
    public class OrderListViewModel
    {
        public int Id { get; set; }
        public List<OrderViewModel> Orders { get; set; }
        public int UserId { get; set; }
    }
}
