using DAL.Priority;

namespace PL.Models
{
    public class QueueForPurchaseViewModel
    {
        public int Id { get; set; }
        public uint Count { get; set; }
        public int GoodsId { get; set; }
        public GoodsViewModel Goods { get; set; } = null!;
        public GoodsInQueuePriority Priority { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
