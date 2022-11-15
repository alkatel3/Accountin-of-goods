using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestProject1
{
    public class ApplicationContextTest
    {
        UnitOfWork UoW = null!;
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            var MockCategorys = new Mock<DbSet<Category>>();
            var MockGoods = new Mock<DbSet<Goods>>();
            var MockContext = new Mock<ApplicationContext>("Data Source=Goods.db");

            MockContext.Setup(m => m.Categories).Returns(MockCategorys.Object);
            MockContext.Setup(m => m.Goods).Returns(MockGoods.Object);

            UoW = new(MockContext.Object);
            var c = new Category() { Name = "A" };
            UoW.Categories.Creat(c);
            UoW.Save();
            var goods = new Goods() { Name = "B", Category = c, Count = 100, Priñe = 10 };
            UoW.Goods.Creat(goods);
            UoW.Save();
            MockCategorys.Verify(m => m.Add(It.IsAny<Category>()), Times.Once());
            MockGoods.Verify(m => m.Add(It.IsAny<Goods>()), Times.Once());

            //MockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Test]
        public void GetAllBlogs_orders_by_name()
        {
            var data = new List<Category>
            {
                new Category { Name = "BBB" },
                new Category { Name = "ZZZ" },
                new Category { Name = "AAA" },
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Category>>();
            mockSet.As<IQueryable<Category>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Category>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Category>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            var mockContext = new Mock<ApplicationContext>("Data Source=Goods.db");
            mockContext.Setup(c => c.Categories).Returns(mockSet.Object);

            UoW = new(mockContext.Object);
            var categories = UoW.Categories.GetAll().ToList();

            Assert.AreEqual(3, categories.Count());
            Assert.AreEqual("BBB", categories[0].Name);
            Assert.AreEqual("ZZZ", categories[1].Name);
            Assert.AreEqual("AAA", categories[2].Name);
        }
    }
}
