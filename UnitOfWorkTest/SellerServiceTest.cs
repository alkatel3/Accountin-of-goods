using BLL.DTO;
using BLL.Servises;

namespace Tests
{
    public class SellerServiceTest
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

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer("test.db");
            MockContext = new Mock<ApplicationContext>(optionsBuilder.Options);
        }

        [Test]
        public void GetAllGoods_TryGetAllGoods()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Price=1, Version=new byte[8] },
                new Goods() { Name="Any", Price=2, Version=new byte[8] },
                new Goods() { Name="Any", Price=3, Version=new byte[8] }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetAllGoods();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void CreateGoods_TryCreatNewGoods_MethodAddFromMockOfApplicationContextForGoodsCalledOnce()
        {
            MockContext.Setup(m => m.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerservice = new SellerServise(UoW);
            var goodsDTO = new GoodsDTO()
            {
                Name = "B",
                Price = 10
            };

            sellerservice.CreateGoods(goodsDTO);

            MockGoods.Verify(m => m.Add(It.Is<Goods>(
                g => g.Name == goodsDTO.Name || g.Price == goodsDTO.Price)), Times.Once());
        }

        [Test]
        public void AddGoods_TryAddGoodsToExistingGoods()
        {
            var goods = new Goods()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsInStock = new GoodsInStock() { Count = 10 },
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };
            var goodsDTO = new GoodsDTO()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsInStock = new GoodsInStockDTO() { Count = 10 },
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };

            MockGoods.Setup(m => m.Find(0)).Returns(goods);
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerService = new SellerServise(UoW);
            goods.GoodsInStock.Count += 10;
            sellerService.AddGoods(goodsDTO, 10);

            MockGoods.Verify(m => m.Update(It.Is<Goods>(g => g.GoodsInStock.Count == 20)), Times.Once);
        }

        [Test]
        public void AddGoods_TryAddNullGoods()
        {
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerService = new SellerServise(UoW);

            using var monitor = sellerService.Monitor();

            sellerService.AddGoods(null, 10);

            monitor.Should().Raise("EntityDidntFind").WithSender(sellerService);
        }

        [Test]
        public void AddGoods_TryAddNotСreatedGoods()
        {
            var goodsDTO = new GoodsDTO()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsInStock = new GoodsInStockDTO() { Count = 10 },
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };

            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var sellerService = new SellerServise(UoW);

            sellerService.AddGoods(goodsDTO, 10);

            MockGoods.Verify(m => m.Update(It.Is<Goods>(g => g.GoodsInStock.Count == 20)), Times.Once);
        }

        [Test]
        public void AddGoods_TryAddZeroGoods()
        {
            var goodsDTO = new GoodsDTO()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsInStock = new GoodsInStockDTO() { Count = 10 },
                GoodsStatus = DAL.Statuses.GoodsStatus.InStock
            };
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerService = new SellerServise(UoW);

            sellerService.AddGoods(goodsDTO, 0);

            MockGoods.Verify(m => m.Update(It.IsAny<Goods>()), Times.Never);
        }

        [Test]
        public void AddGoods_TryAddGoodsToNotAvailableGoods()
        {
            var goodsDTO = new GoodsDTO()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable
            };
            var goods = new Goods()
            {
                Name = "Any",
                Price = 1,
                Id = 0,
                GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable
            };
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            MockGoods.Setup(m => m.Find(0)).Returns(goods);
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var sellerService = new SellerServise(UoW);

            sellerService.AddGoods(goodsDTO, 10);

            MockGoods.Verify(m => m.Update(
                It.Is<Goods>(g => g.GoodsStatus == DAL.Statuses.GoodsStatus.InStock)),
                Times.Once);
            MockGoodsInStock.Verify(m => m.Add(It.Is<GoodsInStock>(g => g.Count == 10)), Times.Once);
        }

        [Test]
        public void GetCurrentGoods_TryGetGoodsById_GotGoodsWithSetId()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Price=1, Id=1 },
                new Goods() { Name="Any", Price=2, Id=2 },
                new Goods() { Name="Any", Price=3, Id=3 }
            };
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockGoods.Setup(m => m.Find(data[2].Id)).Returns(data[2]);
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetCurrentGoods(3);

            goods.Id.Should().Be(data[2].Id);
        }

        [Test]
        public void GetCurrentGoods_TryGetMissingGoods_InvokedGoodsNullEvent()
        {
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);
            using var monitor = sellerServise.Monitor();

            var goods = sellerServise.GetCurrentGoods(3);

            monitor.Should().Raise("EntityDidntFind").WithSender(sellerServise);
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
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetGoodsInStocks();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetOrderLists_TryGetOrderLists()
        {
            var data = new List<OrderList>
            {
                new OrderList(){ Id=1, UserId=1, Version=new byte[8] },
                new OrderList(){ Id=2, UserId=2, Version=new byte[8] },
                new OrderList(){ Id=3, UserId=1, Version=new byte[8] },
            };
            MockOrderList.As<IQueryable<OrderList>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetOrderLists();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetQueueForPurchase_TryGetAllQueueForPurchase()
        {
            var data = new List<QueueForPurchase>
            {
                new QueueForPurchase(){ Goods = new Goods() { Name="Any", Price=1, GoodsInStock = new GoodsInStock(){ Count=1, Version=new byte[8] }, Version=new byte[8] }, Count=3, Version=new byte[8] },
                new QueueForPurchase(){ Goods = new Goods() { Name="Any", Price=2, GoodsInStock = new GoodsInStock(){ Count=2, Version=new byte[8] }, Version=new byte[8] }, Count=2, Version=new byte[8] },
                new QueueForPurchase(){ Goods = new Goods() { Name="Any", Price=3, GoodsInStock = new GoodsInStock(){ Count=3, Version=new byte[8] }, Version=new byte[8] }, Count=1, Version=new byte[8] }
            }.AsQueryable();
            MockQueueForPurchase.As<IQueryable<QueueForPurchase>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetQueueForPurchase();

            goods.Count.Should().Be(3);
            goods.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GetCurrentOrderList_TryGetOrderListById_GotOrderListWithSetId()
        {
            var data = new List<OrderList>
            {
                new OrderList(){ Id=1, UserId=1 },
                new OrderList(){ Id=2, UserId=2 },
                new OrderList(){ Id=3, UserId=1 },
            };
            MockOrderList.As<IQueryable<OrderList>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockOrderList.Setup(m => m.Find(data[2].Id)).Returns(data[2]);
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var goods = sellerServise.GetCurrentOrderList(data[2].Id);

            goods.Id.Should().Be(data[2].Id);
        }

        [Test]
        public void GetCurrentOrderList_TryGetMissingOrderList_InvokedOrderListNullEvent()
        {
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);
            using var monitor = sellerServise.Monitor();

            var goods = sellerServise.GetCurrentOrderList(3);

            monitor.Should().Raise("EntityDidntFind").WithSender(sellerServise);
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
                UserStatus = DAL.Statuses.UserStatus.Seller,
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
            var sellerServise = new SellerServise(UoW);

            var GetUser = sellerServise.CreatAccount(userDTO);

            MockUser.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            GetUser.FirstName.Should().Be(userDTO.FirstName);
            GetUser.LastName.Should().Be(userDTO.LastName);
            GetUser.PhoneNumber.Should().Be(userDTO.PhoneNumber);
            GetUser.UserStatus.Should().Be(userDTO.UserStatus);
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
            var sellerServise = new SellerServise(UoW);

            var GetUser = sellerServise.GetAccount(user.PhoneNumber);

            GetUser.FirstName.Should().Be(user.FirstName);
            GetUser.LastName.Should().Be(user.LastName);
            GetUser.PhoneNumber.Should().Be(user.PhoneNumber);
        }

        [Test]
        public void GetOrders_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Order>
            {
                new Order(){ Goods=new Goods() { Name="Any", Price=1, Version=new byte[8] }, Count=1, Version=new byte[8] },
                new Order(){ Goods=new Goods() { Name="Any", Price=2, Version=new byte[8] }, Count=2, Version=new byte[8] },
                new Order(){ Goods=new Goods() { Name="Any", Price=3, Version=new byte[8] }, Count=3, Version=new byte[8] }
            }.AsQueryable();
            MockOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);
            var sellerServise = new SellerServise(UoW);

            var orders = sellerServise.GetOrders();

            orders.Count.Should().Be(3);
            orders.Should().BeEquivalentTo(data);
        }

        [Test]
        public void UpdateGoods_TryUpdateOneGoodsSetNewGoods_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var goods = new GoodsDTO()
            {
                Name = "Any",
                Price = 10
            };
            var sellerServise = new SellerServise(UoW);

            sellerServise.UpdateGoods(goods);

            MockGoods.Verify(m => 
            m.Update(It.Is<Goods>(g=>g.Name==goods.Name ||
                               g.Price == goods.Price)), Times.Once());
        }

        [Test]
        public void UpdateGoods_TryUpdateOneGoodsSetNewGoodsInStock_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var goodsInStock = new GoodsInStockDTO()
            {
                GoodsId=1,
                Count=10,
            };
            var sellerServise = new SellerServise(UoW);

            sellerServise.UpdateGoods(goodsInStock);

            MockGoodsInStock.Verify(m =>
            m.Update(It.Is<GoodsInStock>(g => g.GoodsId == goodsInStock.GoodsId ||
                                    g.Count == goodsInStock.Count)), Times.Once());
        }

        [Test]
        public void BringGoodsFromQueueForPurchase_()
        {
            var queueForPurchaseDTO = new QueueForPurchaseDTO()
            {
                Goods = new GoodsDTO()
                {
                    GoodsInStock = new GoodsInStockDTO()
                    {
                        Count = 10,
                        GoodsId = 1
                    },
                    Id = 1,
                    Name = "Any",
                    Price = 10
                },
                Id=1,
                Count = 3,
                GoodsId=1
            };
            var queueForPurchaseData = new QueueForPurchase() { Id = 1 };
            var goodsInStockData = new List<GoodsInStock>()
            {
                new GoodsInStock()
                {
                    Count=queueForPurchaseDTO.Goods.GoodsInStock.Count,
                    GoodsId=queueForPurchaseDTO.Goods.GoodsInStock.GoodsId,
                    Goods=new Goods()
                }
            };
            var OrderData = new List<Order>{
                new Order()
                {
                    GoodsId = 1,
                    OrderStatus = DAL.Statuses.OrderStatus.WaitingForDelivery
                }
            };

            MockGoodsInStock.As<IQueryable<GoodsInStock>>().Setup(m => m.GetEnumerator())
                .Returns(() => goodsInStockData.GetEnumerator());
            MockOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator())
                .Returns(() => OrderData.GetEnumerator());
            MockQueueForPurchase.Setup(m => m.Find(1))
                .Returns(() => queueForPurchaseData);
            MockContext.Setup(c => c.Set<GoodsInStock>())
                .Returns(MockGoodsInStock.Object);
            MockContext.Setup(c => c.Set<Order>())
                .Returns(MockOrder.Object);
            MockContext.Setup(c => c.Set<Goods>())
                .Returns(MockGoods.Object);
            MockContext.Setup(c => c.Set<QueueForPurchase>())
                .Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);
            var expectedCountForGoodsInStock = queueForPurchaseDTO.Goods.GoodsInStock.Count + queueForPurchaseDTO.Count;

            var sellerServise = new SellerServise(UoW);

            sellerServise.BringGoodsFromQueueForPurchase(queueForPurchaseDTO);

            MockGoodsInStock.Verify(m =>
            m.Update(It.Is<GoodsInStock>(g=>g.Count== expectedCountForGoodsInStock)), Times.Once());
            MockOrder.Verify(m =>
            m.Update(It.Is<Order>(o=>o.OrderStatus == DAL.Statuses.OrderStatus.AwaitingShipment)), Times.Once);
            MockQueueForPurchase.Verify(m =>
            m.Remove(It.Is<QueueForPurchase>(q => q.Id == queueForPurchaseDTO.Id)), Times.Once);
        }
    }
}
