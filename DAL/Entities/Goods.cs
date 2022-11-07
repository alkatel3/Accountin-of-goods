namespace DAL.Entities
{
    public class Goods
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Count { get; set; }
        public decimal Priсe { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}