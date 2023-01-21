using DAL.Priority;

namespace BLL.DTO
{
    public class QueueForPurchaseDTO
    {
        public int Id { get; set; }
        public uint Count { get; set; }
        public int GoodsId { get; set; }
        public GoodsDTO Goods { get; set; } = null!;
        public GoodsInQueuePriority Priority { get; set; }
        public byte[] Version { get; set; } = null!;
    }
}
