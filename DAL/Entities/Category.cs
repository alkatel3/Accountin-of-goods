namespace DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<Goods> Goods { get; set; }

        public Category()
        {
            Goods = new List<Goods>();
        }
    }
}