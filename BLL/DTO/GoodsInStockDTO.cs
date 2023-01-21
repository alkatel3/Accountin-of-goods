namespace BLL.DTO
{
    public class GoodsInStockDTO
    {
        public int Id { get; set; }
        public uint Count { get; set; }
        public int GoodsId { get; set; }
        public GoodsDTO Goods { get; set; } = null!;
        public byte[] Version { get; set; } = null!;
    }
}
