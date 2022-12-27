using DAL.Statuses;

namespace DAL.Entities
{
    public class User : BaseEntity<int>
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public UserStatus UserStatus { get; set; }
        public OrderList OrderList { get; set; }
    }
}
