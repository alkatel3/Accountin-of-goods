namespace BLL.DTO
{
    public class OrderListDTO
    {
        public int Id { get; set; }
        public List<OrderDTO> Orders { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }
        public byte[] Version { get; set; }

        public OrderListDTO()
        {
            Orders = new List<OrderDTO>();
        }
    }
}
