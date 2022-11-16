using Microsoft.EntityFrameworkCore;
using Moq;
using UoW.Repositories;

namespace TestProject1
{
    public class ApplicationContextTest
    {
        UnitOfWork UoW = null!;
        Mock<DbSet<Category>> MockCategories = null!;
        Mock<DbSet<Goods>> MockGoods=null!;
        Mock<ApplicationContext> MockContext = null!;
        [SetUp]
        public void Setup()
        {
            MockCategories = new Mock<DbSet<Category>>();
            MockGoods = new Mock<DbSet<Goods>>();
            MockContext = new Mock<ApplicationContext>("test.db");
        }

        [TearDown]
        public void Finalize()
        {
            UoW.Dispose();
        }

        [Test]
        public void CategoriesCreat_TryCreatNewCategory_MethodAddFromMockOfApplicationContextForCategoryCalledOnce()
        {
            MockContext.Setup(m => m.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            var category = new Category() { Name = "A" };

            UoW.Categories.Creat(category);

            MockCategories.Verify(m => m.Add(category), Times.Once());
        }

        [Test]
        public void GoodsCreat_TryCreatNewGoods_MethodAddFromMockOfApplicationContextForGoodsCalledOnce()
        {
            MockContext.Setup(m => m.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods() { Name = "B",
                Category =new Category() { Name = "category" },
                Count = 100, Priñe = 10 };

            UoW.Goods.Creat(goods);

            MockGoods.Verify(m => m.Add(It.IsAny<Goods>()), Times.Once());
        }

        [Test]
        public void CategoriesGet_TryGetOneCategoryById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.Categories.Get(0);

            MockCategories.Verify(m => m.Find(0), Times.Once());
        }

        [Test]
        public void GoodsGet_TryGetOneGoodsById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.Goods.Get(0);

            MockGoods.Verify(m => m.Find(0), Times.Once());
        }

        [Test]
        public void Save_TrySaveMockDB_MethodSaveChangesFromMockOfApplicationContextCalledOnce()
        {
            UoW = new(MockContext.Object);

            UoW.Save();

            MockContext.Verify(m => m.SaveChanges(), Times.Once());
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
