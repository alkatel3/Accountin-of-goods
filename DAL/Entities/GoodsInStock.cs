namespace DAL.Entities
{
    public class GoodsInStock : BaseEntity<int>
    {
        public uint Count { get; set; }
        public int GoodsId { get; set; }
        public Goods Goods { get; set; }
    }
}
