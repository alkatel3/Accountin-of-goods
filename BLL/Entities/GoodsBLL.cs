namespace BLL.Entities
{
    public class GoodsBLL
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Count { get; set; }
        public decimal Priсe { get; set; }
        public CategoryBLL CategoryBLL { get; set; } = null!;
    }
}
