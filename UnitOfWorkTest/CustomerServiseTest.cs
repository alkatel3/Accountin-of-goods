using BLL.DTO;
using BLL.Servises;

namespace Tests
{
    public class CustomerServiseTest
    {
        UnitOfWork UoW = null!;
        Mock<DbSet<GoodsInStock>> MockGoodsInStock = null!;
        Mock<DbSet<Goods>> MockGoods = null!;
        Mock<DbSet<Order>> MockOrder = null!;
        Mock<DbSet<OrderList>> MockOrderList = null!;
        Mock<DbSet<QueueForPurchase>> MockQueueForPurchase = null!;
        Mock<DbSet<User>> MockUser = null!;
        Mock<ApplicationContext> MockContext = null!;
        [SetUp]
        public void Setup()
        {
            MockGoodsInStock = new Mock<DbSet<GoodsInStock>>();
            MockGoods = new Mock<DbSet<Goods>>();
            MockOrder = new Mock<DbSet<Order>>();
            MockUser = new Mock<DbSet<User>>();
            MockQueueForPurchase = new Mock<DbSet<QueueForPurchase>>();
            MockOrderList = new Mock<DbSet<OrderList>>();
            MockContext = new Mock<ApplicationContext>("test.db");
        }

        [Test]
        public void CreatAccount_TryCreatNewAccount_MethodAddFromMockOfUserCalledOnce()
        {
            UserDTO userDTO = new UserDTO()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 123,
            };
            User user = new User()
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                PhoneNumber = userDTO.PhoneNumber,
                UserStatus = DAL.Statuses.UserStatus.Customer,
            };
            var predicate = (User g) => g.PhoneNumber == userDTO.PhoneNumber;
            var data = new List<User>
            {
                new User() { FirstName="Any", LastName="Any", PhoneNumber=1 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=2 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=3 }
            };
            var expected = data.Where(predicate).ToList();
            MockUser.Setup(m => m.Add(It.IsAny<User>())).Callback(() => data.Add(user));
            MockUser.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            var GetUser = customerServise.CreatAccount(userDTO);

            MockUser.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            GetUser.FirstName.Should().Be(userDTO.FirstName);
            GetUser.LastName.Should().Be(userDTO.LastName);
            GetUser.PhoneNumber.Should().Be(userDTO.PhoneNumber);
            GetUser.UserStatus.Should().Be(userDTO.UserStatus);
        }

        [Test]
        public void CreatOrder_TryCreatNewOrder_MethodAddFromMockOfOrderCalledOnce()
        {
            UserDTO client = new UserDTO
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 123,
                OrderList = new OrderListDTO()
            };
            GoodsDTO goods = new GoodsDTO()
            {
                Name = "Any",
                Priсe = 10,
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };
            uint count = 10;
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            customerServise.CreatOrder(client, goods, count);

            MockOrder.Verify(m => m.Add(It.Is<Order>(o => o.Count == 10)));
        }

        [Test]
        public void GetAccount_TryGetAccount_MethodAddFromMockOfUserCalledOnce()
        {
            User user = new User()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 123,
            };
            var predicate = (User g) => g.PhoneNumber == user.PhoneNumber;
            var data = new List<User>
            {
                new User() { FirstName="Any", LastName="Any", PhoneNumber=1 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=2 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=3 },
                user
            };
            var expected = data.Where(predicate).ToList();
            MockUser.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            var GetUser = customerServise.GetAccount(user.PhoneNumber);

            GetUser.FirstName.Should().Be(user.FirstName);
            GetUser.LastName.Should().Be(user.LastName);
            GetUser.PhoneNumber.Should().Be(user.PhoneNumber);
        }

        [Test]
        public void CreatOrder_TryCreatNewOrderWithNullGoods_InvokedGoodsNullEvent()
        {
            GoodsDTO goods = new GoodsDTO()
            {
                Name = "Any",
                Priсe = 10,
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };
            uint count = 10;
            var customerServise = new CustomerServise(UoW);
            using var monitor = customerServise.Monitor();

            customerServise.CreatOrder(null, goods, count);

            monitor.Should().Raise("ClientNull").WithSender(customerServise);
        }

        [Test]
        public void CreatOrder_TryCreatNewOrderWithNullClient_InvokedClientNullEvent()
        {
            UserDTO client = new UserDTO
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 123,
                OrderList = new OrderListDTO()
            };
            uint count = 10;
            var customerServise = new CustomerServise(UoW);
            using var monitor = customerServise.Monitor();

            customerServise.CreatOrder(client, null, count);

            monitor.Should().Raise("GoodsNull").WithSender(customerServise);
        }

        [Test]
        public void CreatOrder_TryCreatNewOrderWithGoodsMissingGoods_AddGoodsToQueueForPurchase()
        {
            UserDTO client = new UserDTO
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 123,
                OrderList = new OrderListDTO()
            };
            GoodsDTO goods = new GoodsDTO()
            {
                Name = "Any",
                Priсe = 10,
                GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable
            };
            uint count = 10;
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            customerServise.CreatOrder(client, goods, count);

            MockQueueForPurchase.Verify(m => m.Add(It.Is<QueueForPurchase>(o => o.Count == 10)));
        }

        [Test]
        public void GetAllGoods_TryGetAllGoods()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Priсe=1, Version=new byte[8] },
                new Goods() { Name="Any", Priсe=2, Version=new byte[8] },
                new Goods() { Name="Any", Priсe=3, Version=new byte[8] }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            var goods = customerServise.GetAllGoods();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetCurrentGoods_TryGetGoodsById_GotGoodsWithSetId()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Priсe=1, Id=1 },
                new Goods() { Name="Any", Priсe=2, Id=2 },
                new Goods() { Name="Any", Priсe=3, Id=3 }
            };
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockGoods.Setup(m => m.Find(data[2].Id)).Returns(data[2]);
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            var goods = customerServise.GetCurrentGoods(3);

            goods.Id.Should().Be(data[2].Id);
        }

        [Test]
        public void GetCurrentGoods_TryGetMissingGoods_InvokedGoodsNullEvent()
        {
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);
            using var monitor = customerServise.Monitor();

            var goods = customerServise.GetCurrentGoods(3);

            monitor.Should().Raise("GoodsNull").WithSender(customerServise);
        }

        [Test]
        public void GetGoodsInStock_TryGetGoodsInStock()
        {
            var data = new List<GoodsInStock>
            {
                new GoodsInStock(){ Count = 1, Version=new byte[8] },
                new GoodsInStock(){ Count = 2, Version=new byte[8] },
                new GoodsInStock(){ Count = 3, Version=new byte[8] }
            }.AsQueryable();
            MockGoodsInStock.As<IQueryable<GoodsInStock>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var customerServise = new CustomerServise(UoW);

            var goods = customerServise.GetGoodsInStock();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }
    }
}
