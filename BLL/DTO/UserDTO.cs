using DAL.Statuses;

namespace BLL.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int PhoneNumber { get; set; }
        public UserStatus UserStatus { get; set; }
        public OrderListDTO OrderList { get; set; }
        public byte[] Version { get; set; }
    }
}
