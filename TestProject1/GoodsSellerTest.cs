using BLL;
using BLL.Entities;

namespace TestProject1
{
    public class GoodsSellerTest
    {
        EFUnitOfWork UoW = null!;
        Mock<DbSet<Category>> MockCategories = null!;
        Mock<DbSet<Goods>> MockGoods = null!;
        Mock<ApplicationContext> MockContext = null!;
        GoodsSeller seller = null!;
        GoodsController controller = null!;
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
            controller.Dispose();
        }

        [Test]
        public void Sell_SellTenGoodsFromModelDb_CountOfGoodsMinusTen()
        {
            var goods = new GoodsBLL()
            {
                Name = "Any",
                CategoryBLL = new CategoryBLL() { Name = "category", Id = 10 },
                Count = 100,
                Priсe = 10,
                Id = 10
            };
            MockCategories.Setup(m => m.Find(goods.CategoryBLL.Id)).Returns(new Category() { Name = goods.CategoryBLL.Name, Id = goods.CategoryBLL.Id });
            MockContext.Setup(m => m.Categories).Returns(MockCategories.Object);
            MockGoods.Setup(m => m.Find(goods.Id)).Returns(new Goods()
            {
                Name = goods.Name,
                Category = new Category() { Name = goods.CategoryBLL.Name, Id = goods.CategoryBLL.Id },
                Priсe = goods.Priсe,
                Count = goods.Count,
                Id = goods.Id
            });
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new GoodsController(UoW);
            seller = new(controller);

            seller.Sell(10, goods);

            MockGoods.Verify(m => m.Update(It.Is<Goods>(g =>
                g.Name == goods.Name &&
                g.Count == goods.Count &&
                g.Priсe == goods.Priсe &&
                g.Category.Name == goods.CategoryBLL.Name
                )), Times.Once());
            goods.Count.Should().Be(90);
        }
    }
}
