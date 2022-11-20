namespace TestProject1
{
    public class ApplicationContextTest
    {
        EFUnitOfWork UoW = null!;
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

            MockGoods.Verify(m => m.Add(goods), Times.Once());
        }

        [Test]
        public void CategoriesGet_TryGetOneCategoryById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var c = new Category() { Name = "123", Id=10 };
            MockCategories.Setup(c => c.Find(10)).Returns(c);
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.Categories.Get(10);

            Actual.Should().Be(c);
        }

        [Test]
        public void GoodsGet_TryGetOneGoodsById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var g = new Goods()
            {
                Name = "Any",
                Priñe = 10,
                Count = 10,
                Id = 10
            };
            MockGoods.Setup(m => m.Find(10)).Returns(g);
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.Goods.Get(10);

            Actual.Should().Be(g);
        }

        [Test]
        public void CategoriesUpdate_TryUpdateOneCategoryById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            var c = new Category() { Name = "123" };

            UoW.Categories.Update(c);

            MockCategories.Verify(m => m.Update(c), Times.Once());
        }

        [Test]
        public void GoodsUpdate_TryUpdateOneGoodsById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var g = new Goods()
            {
                Name = "Any",
                Priñe = 10,
                Count = 10,
            };

            UoW.Goods.Update(g);

            MockGoods.Verify(m => m.Update(g), Times.Once());
        }

        [Test]
        public void CategoryDelete_TryDeleteOneCategoryById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var c = new Category() { Name = "123", Id=10 };
            MockCategories.Setup(m => m.Find(10)).Returns(c);
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);

            UoW.Categories.Delete(10);

            MockCategories.Verify(m => m.Remove(c), Times.Once());
        }

        [Test]
        public void GoodsDelete_TryDeleteOneGoodsById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var g = new Goods()
            {
                Name = "Any",
                Priñe = 10,
                Count = 10,
                Id=10
            };
            MockGoods.Setup(m=>m.Find(10)).Returns(g);
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            UoW.Goods.Delete(10);

            MockGoods.Verify(m => m.Remove(g), Times.Once());
        }

        [Test]
        public void CategoriesGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Category>
            {
                new Category { Name = "BBB" },
                new Category { Name = "ZZZ" },
                new Category { Name = "AAA" },
            }.AsQueryable();
            MockCategories.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);

            var goods = UoW.Categories.GetAll().ToList();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GoodsGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Count=1, Priñe=1 },
                new Goods() { Name="Any", Count=2, Priñe=2 },
                new Goods() { Name="Any", Count=3, Priñe=3 }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.Goods.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GoodsFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Count=-1, Priñe=1 },
                new Goods() { Name="Any", Count=2, Priñe=2 },
                new Goods() { Name="Any", Count=3, Priñe=3 }
            }.AsQueryable();
            var expected = new List<Goods>
            {
                data.ToList()[1],
                data.ToList()[2]
            };

            var predicate = (Goods g) => g.Count > 0;
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Goods).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.Goods.Find(predicate).ToList();

            actual.Count.Should().Be(2);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void CategoriesFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Category>
            {
                new Category() { Name="Any", Id=1 },
                new Category() { Name="Any", Id=2 },
                new Category() { Name="Any", Id=3 }
            }.AsQueryable();
            var expected = new List<Category>
            {
                data.ToList()[1]
            };

            var predicate = (Category g) => g.Id==2;
            MockCategories.As<IQueryable<Category>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.Categories.Find(predicate).ToList();

            actual.Count.Should().Be(1);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Save_TrySaveMockDB_MethodSaveChangesFromMockOfApplicationContextCalledOnce()
        {
            UoW = new(MockContext.Object);

            UoW.Save();

            MockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}
