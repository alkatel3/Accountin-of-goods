using DAL.Statuses;

namespace BLL.DTO
{
    public class GoodsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public GoodsStatus GoodsStatus { get; set; }
        public ICollection<OrderDTO> Orders { get; set; } = null!;
        public QueueForPurchaseDTO QueueForPurchase { get; set; } = null!;
        public GoodsInStockDTO GoodsInStock { get; set; } = null!;
        public byte[] Version { get; set; } = null!;

        public GoodsDTO()
        {
            Orders = new List<OrderDTO>();
        }
    }
}
