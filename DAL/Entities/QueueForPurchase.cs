using DAL.Priority;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class QueueForPurchase:BaseEntity<int>
    {
        public uint Count { get; set; }
        public int GoodsId { get; set; }
        public Goods Goods { get; set; }
        public GoodsInQueuePriority Priority { get; set; }
    }
}
