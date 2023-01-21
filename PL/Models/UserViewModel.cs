using DAL.Statuses;

namespace PL.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public UserStatus UserStatus { get; set; }
        public OrderListViewModel OrderList { get; set; }
    }
}
