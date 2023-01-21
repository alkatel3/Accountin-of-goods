using BLL.DTO;
using DAL.Statuses;

namespace PL.Models
{
    public class GoodsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public GoodsStatus GoodsStatus { get; set; }
        public GoodsInStockDTO GoodsInStock { get; set; }
    }
}
