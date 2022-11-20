using BLL;
using BLL.Entities;

namespace TestProject1
{
    public class CategoryControllerTest
    {
        EFUnitOfWork UoW = null!;
        Mock<DbSet<Category>> MockCategories = null!;
        Mock<DbSet<Goods>> MockGoods = null!;
        Mock<ApplicationContext> MockContext = null!;
        CategoryController controller = null!;
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
        public void Creat_TryCreatNewCategoryInDb_ResultIsCalledAddForCategoriesDbSet()
        {
            MockContext.Setup(m => m.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            controller = new CategoryController(UoW);
            var category = new CategoryBLL() { Name = "A" };

            controller.Creat(category);

            MockCategories.Verify(m => m.Add(It.Is<Category>(c=>c.Name==category.Name)), Times.Once());
        }

        [Test]
        public void Delete_TryDeleteOneCategoryById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var category = new CategoryBLL() { Name = "123", Id = 10 };
            MockCategories.Setup(m => m.Find(10)).Returns(new Category() { Id=category.Id,Name=category.Name});
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            controller = new CategoryController(UoW);

            controller.Delete(category);

            MockCategories.Verify(m => m.Remove(It.Is<Category>(c => c.Name == category.Name && c.Id==category.Id)), Times.Once());
        }

        [Test]
        public void UpDate_TryUpdateOneCategoryById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            controller = new CategoryController(UoW);
            var category = new CategoryBLL() { Name = "123", Id=1 };

            controller.UpDate(category);

            MockCategories.Verify(m => m.Update(It.Is<Category>(c => c.Name == category.Name && c.Id == category.Id)), Times.Once());
        }

        [Test]
        public void GetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
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
            controller = new(UoW);

            var goods = controller.GetAll();

            goods.Count.Should().Be(3);
            goods[0].Name.Should().Be(data.ToList<Category>()[0].Name);
            goods[1].Name.Should().Be(data.ToList<Category>()[1].Name);
            goods[2].Name.Should().Be(data.ToList<Category>()[2].Name);
        }

        [Test]
        public void GetCurrent_TryGetOneCategoryById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var c = new CategoryBLL() { Name = "123", Id = 10 };
            MockCategories.Setup(c => c.Find(10)).Returns(new Category() { Name=c.Name, Id=c.Id});
            MockContext.Setup(c => c.Categories).Returns(MockCategories.Object);
            UoW = new(MockContext.Object);
            controller = new CategoryController(UoW);

            var Actual = controller.GetCurrent(c.Id);

            Actual.Name.Should().Be(c.Name);
            Actual.Id.Should().Be(c.Id);
        }
    }
}
