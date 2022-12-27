using DAL.Statuses;

namespace BLL.DTO
{
    public class GoodsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Priсe { get; set; }
        public GoodsStatus GoodsStatus { get; set; }
        public ICollection<OrderDTO> Orders { get; set; }
        public QueueForPurchaseDTO QueueForPurchase { get; set; }
        public GoodsInStockDTO GoodsInStock { get; set; }
        public byte[] Version { get; set;  }

        public GoodsDTO()
        {
            Orders = new List<OrderDTO>();
        }
    }
}
