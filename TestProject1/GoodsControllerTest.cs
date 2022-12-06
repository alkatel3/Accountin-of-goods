using BLL;
using BLL.Entities;

namespace TestProject1
{
    public class GoodsControllerTest
    {
        EFUnitOfWork UoW = null!;
        Mock<DbSet<Category>> MockCategories = null!;
        Mock<DbSet<Goods>> MockGoods = null!;
        Mock<ApplicationContext> MockContext = null!;
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
        public void Creat_TryCreatNewGoods_MethodAddFromMockOfApplicationContextForGoodsCalledOnce()
        {
            var goods = new GoodsBLL()
            {
                Name = "B",
                CategoryBLL=new CategoryBLL() { Name = "category", Id=10 },
                Count = 100,
                Priсe = 10
            };
            MockCategories.Setup(m => m.Find(goods.CategoryBLL.Id)).Returns(new Category() { Name = goods.CategoryBLL.Name, Id = goods.CategoryBLL.Id });
            MockContext.Setup(m => m.Categories).Returns(MockCategories.Object);
            MockContext.Setup(m => m.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);


            controller.Creat(goods);

            MockGoods.Verify(m => m.Add(It.Is<Goods>(g=>
                g.Name==goods.Name && 
                g.Count==goods.Count &&
                g.Priсe==goods.Priсe &&
                g.Category.Name==goods.CategoryBLL.Name
                )), Times.Once());
        }

        [Test]
        public void Delete_TryDeleteOneGoodsById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var goods = new GoodsBLL()
            {
                Name = "Any",
                Priсe = 10,
                Count = 10,
                Id = 10
            };
            MockGoods.Setup(m => m.Find(goods.Id)).Returns(new Goods()
            {
                Name = goods.Name,
                Priсe = goods.Priсe,
                Count=goods.Count,
                Id=goods.Id
            });
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);

            controller.Delete(goods);

            MockGoods.Verify(m => m.Remove(It.Is<Goods>(g=>
                g.Name==goods.Name &&
                g.Priсe==goods.Priсe &&
                g.Count==goods.Count &&
                g.Id==goods.Id)
                ), Times.Once());
        }

        [Test]
        public void UpDate_TryUpdateOneGoodsById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            var goods = new GoodsBLL()
            {
                Name = "B",
                CategoryBLL = new CategoryBLL() { Name = "category", Id = 10 },
                Count = 100,
                Priсe = 10,
                Id=10
            };
            MockCategories.Setup(m => m.Find(goods.CategoryBLL.Id)).Returns(new Category() { Name = goods.CategoryBLL.Name, Id = goods.CategoryBLL.Id });
            MockContext.Setup(m => m.Categories).Returns(MockCategories.Object);
            MockGoods.Setup(m=>m.Find(goods.Id)).Returns(new Goods()
            {
                Name = goods.Name,
                Category=new Category() { Name=goods.CategoryBLL.Name, Id=goods.CategoryBLL.Id},
                Priсe = goods.Priсe,
                Count = goods.Count,
                Id = goods.Id
            });
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);

            controller.UpDate(goods);

            MockGoods.Verify(m => m.Update(It.Is<Goods>(g =>
                g.Name == goods.Name &&
                g.Count == goods.Count &&
                g.Priсe == goods.Priсe &&
                g.Category.Name == goods.CategoryBLL.Name
                )), Times.Once());
        }

        [Test]
        public void GetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Count=1, Priсe=1, Id=1 },
                new Goods() { Name="Any", Count=2, Priсe=2, Id=2 },
                new Goods() { Name="Any", Count=3, Priсe=3, Id=3 }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);

            var categories = controller.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories[0].Id.Should().Be(data.ToList<Goods>()[0].Id);
            categories[1].Id.Should().Be(data.ToList<Goods>()[1].Id);
            categories[2].Id.Should().Be(data.ToList<Goods>()[2].Id);
        }

        [Test]
        public void GetAllFollowing_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Count=1, Priсe=1, CategoryId=1 },
                new Goods() { Name="Any", Count=2, Priсe=2, CategoryId=2 },
                new Goods() { Name="Any", Count=3, Priсe=3, CategoryId=2 }
            }.AsQueryable();
            var expected = new List<GoodsBLL>
            {
                new GoodsBLL() {Name="Any",Count=2, Priсe=2, CategoryBLL=new CategoryBLL(){Id=2 }},
                new GoodsBLL() {Name="Any",Count=3, Priсe=3, CategoryBLL=new CategoryBLL(){Id=2 }}
            };

            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);

            var actual = controller.GetAllFollowing(2).ToList();

            actual.Count.Should().Be(2);
            actual[0].Count.Should().Be(expected[0].Count);
            actual[1].Count.Should().Be(expected[1].Count);
        }

        [Test]
        public void GetInStock_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Count=-1, Priсe=1},
                new Goods() { Name="Any", Count=2, Priсe=2},
                new Goods() { Name="Any", Count=3, Priсe=3}
            }.AsQueryable();
            var expected = new List<GoodsBLL>
            {
                new GoodsBLL() {Name="Any",Count=2, Priсe=2},
                new GoodsBLL() {Name="Any",Count=3, Priсe=3}
            };

            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            controller = new(UoW);

            var actual = controller.GetInStock().ToList();

            actual.Count.Should().Be(2);
            actual[0].Count.Should().Be(expected[0].Count);
            actual[1].Count.Should().Be(expected[1].Count);
        }
    }
}
