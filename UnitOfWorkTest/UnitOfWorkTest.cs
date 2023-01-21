using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Tests
{
    public class UnitOfWorkTest
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
        public void GoodsCreat_TryCreatNewGoods_MethodAddFromMockOfApplicationContextForGoodsCalledOnce()
        {
            MockContext.Setup(m => m.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "B",
                Price = 10
            };

            UoW.Goods.Creat(goods);

            MockGoods.Verify(m => m.Add(goods), Times.Once());
        }

        [Test]
        public void QueueForPurchaseCreat_TryCreatNewQueueForPurchase_MethodAddFromMockOfApplicationContextForQueueForPurchaseCalledOnce()
        {
            MockContext.Setup(m => m.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "B",
                Price = 10
            };
            var QueueForPurchase = new QueueForPurchase()
            {
                Goods = goods,
                Count = 10
            };

            UoW.QueueForPurchase.Creat(QueueForPurchase);

            MockQueueForPurchase.Verify(m => m.Add(QueueForPurchase), Times.Once());
        }

        [Test]
        public void GoodsInStockCreat_TryCreatNewGoodsInStock_MethodAddFromMockOfApplicationContextForGoodsInStockCalledOnce()
        {
            MockContext.Setup(m => m.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "B",
                Price = 10
            };
            var goodsInStock = new GoodsInStock()
            {
                Goods = goods,
                Count = 10
            };

            UoW.GoodsInStock.Creat(goodsInStock);

            MockGoodsInStock.Verify(m => m.Add(goodsInStock), Times.Once());
        }

        [Test]
        public void OrderCreat_TryCreatNewOrder_MethodAddFromMockOfApplicationContextForOrderCalledOnce()
        {
            MockContext.Setup(m => m.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "B",
                Price = 10
            };
            var order = new Order()
            {
                Goods = goods,
                Count = 2
            };

            UoW.Orders.Creat(order);

            MockOrder.Verify(m => m.Add(order), Times.Once());
        }

        [Test]
        public void OrderListCreat_TryCreatNewOrderList_MethodAddFromMockOfApplicationContextForOrderListCalledOnce()
        {
            MockContext.Setup(m => m.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);
            var OrderList = new OrderList()
            {
                User = new User { FirstName = "any", LastName = "Any", PhoneNumber = 123 }
            };

            UoW.OrderLists.Creat(OrderList);

            MockOrderList.Verify(m => m.Add(OrderList), Times.Once());
        }

        [Test]
        public void UserCreat_TryCreatNewUser_MethodAddFromMockOfApplicationContextForUserCalledOnce()
        {
            MockContext.Setup(m => m.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);
            var User = new User
            {
                FirstName = "any",
                LastName = "Any",
                PhoneNumber = 123
            };

            UoW.Users.Creat(User);

            MockUser.Verify(m => m.Add(User), Times.Once());
        }

        [Test]
        public void GoodsGet_TryGetOneGoodsById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Price=1 },
                new Goods() { Name="Any", Price=2 },
                new Goods() { Name="Any", Price=3 }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockGoods.Setup(m => m.Find(0)).Returns(data.FirstOrDefault());
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);


            var Actual = UoW.Goods.Get(0);

            Actual.Should().Be(data.FirstOrDefault());
        }

        [Test]
        public void GoodsInStockGet_TryGetOneGoodsInStockById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var data = new List<GoodsInStock>
            {
                new GoodsInStock() { Goods = new Goods() { Name="Any", Price=1 }, Count=1 },
                new GoodsInStock() { Goods = new Goods() { Name="Any", Price=2 }, Count=2 },
                new GoodsInStock() { Goods = new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockGoodsInStock.As<IQueryable<GoodsInStock>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockGoodsInStock.Setup(m => m.Find(0)).Returns(data.FirstOrDefault());
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.GoodsInStock.Get(0);

            Actual.Should().Be(data.FirstOrDefault());
        }

        [Test]
        public void QueueForPurchaseGet_TryGetOneQueueForPurchaseById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var data = new List<QueueForPurchase>
            {
                new QueueForPurchase() { Goods = new Goods() { Name="Any", Price=1 }, Count=1 },
                new QueueForPurchase() { Goods = new Goods() { Name="Any", Price=2 }, Count=2 },
                new QueueForPurchase() { Goods = new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockQueueForPurchase.As<IQueryable<QueueForPurchase>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockQueueForPurchase.Setup(m => m.Find(0)).Returns(data.FirstOrDefault());
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);

            var Actual = UoW.QueueForPurchase.Get(0);

            Actual.Should().Be(data.FirstOrDefault());
        }
        
        [Test]
        public void OrderGet_TryGetOneOrderById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var data = new List<Order>
            {
                new Order() { Goods = new Goods() { Name="Any", Price=1 }, Count=1 },
                new Order() { Goods = new Goods() { Name="Any", Price=2 }, Count=2 },
                new Order() { Goods = new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockOrder.Setup(m => m.Find(0)).Returns(data.FirstOrDefault());
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);


            var Actual = UoW.Orders.Get(0);

            Actual.Should().Be(data.FirstOrDefault());
        }

        [Test]
        public void OrderListGet_TryGetOneOrderListById_MethodFindFromMockOfApplicationContextCalledOnce()
        {
            var data = new List<OrderList>
            {
                new OrderList(){User=new User(){ FirstName = "any", LastName = "Any", PhoneNumber = 1} },
                new OrderList(){User=new User(){ FirstName = "any", LastName = "Any", PhoneNumber = 2} },
                new OrderList(){User=new User(){ FirstName = "any", LastName = "Any", PhoneNumber = 3} }
            }.AsQueryable();
            MockOrderList.As<IQueryable<OrderList>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockOrderList.Setup(m => m.Find(0)).Returns(data.FirstOrDefault());
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);


            var Actual = UoW.OrderLists.Get(0);

            Actual.Should().Be(data.FirstOrDefault());
        }

        [Test]
        public void GoodsUpdate_TryUpdateOneGoodsById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10
            };

            UoW.Goods.Update(goods);

            MockGoods.Verify(m => m.Update(goods), Times.Once());
        }

        [Test]
        public void GoodsInStockUpdate_TryUpdateOneGoodsInStockById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10
            };
            var goodsInStock = new GoodsInStock()
            {
                Goods = goods,
                Count = 10
            };

            UoW.GoodsInStock.Update(goodsInStock);

            MockGoodsInStock.Verify(m => m.Update(goodsInStock), Times.Once());
        }

        [Test]
        public void OrderUpdate_TryUpdateOneOrderById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10
            };
            var order = new Order()
            {
                Goods = goods,
                Count = 10
            };
            UoW.Orders.Update(order);

            MockOrder.Verify(m => m.Update(order), Times.Once());
        }

        [Test]
        public void UserUpdate_TryUpdateOneUserById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);
            var user = new User()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 12345
            };
            UoW.Users.Update(user);

            MockUser.Verify(m => m.Update(user), Times.Once());
        }

        [Test]
        public void OrderListUpdate_TryUpdateOneOrderListById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);
            var user = new User()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 12345
            };
            var orderList = new OrderList()
            {
                User = user
            };
            UoW.OrderLists.Update(orderList);

            MockOrderList.Verify(m => m.Update(orderList), Times.Once());
        }

        [Test]
        public void QueueForPurchaseUpdate_TryUpdateOneQueueForPurchaseById_MethodUpdateFromMockOfApplicationContextCalledOnce()
        {
            MockContext.Setup(m => m.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);
            var goods = new Goods()
            {
                Name = "B",
                Price = 10
            };
            var QueueForPurchase = new QueueForPurchase()
            {
                Goods = goods,
                Count = 10
            };

            UoW.QueueForPurchase.Update(QueueForPurchase);

            MockQueueForPurchase.Verify(m => m.Update(QueueForPurchase), Times.Once());
        }

        [Test]
        public void GoodsDelete_TryDeleteOneGoodsById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10,
                Id = 10,
                GoodsStatus=DAL.Statuses.GoodsStatus.NotAvailable
            };
            MockGoods.Setup(m => m.Find(10)).Returns(goods);
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            UoW.Goods.Delete(10);

            MockGoods.Verify(m => m.Remove(goods), Times.Once());
        }

        [Test]
        public void GoodsInStockDelete_TryDeleteOneGoodsInStockById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10,
                Id = 10
            };
            var goodsInStock = new GoodsInStock()
            {
                Goods = goods,
                Count = 10,
                Id = 10
            };
            MockGoodsInStock.Setup(m => m.Find(10)).Returns(goodsInStock);
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);

            UoW.GoodsInStock.Delete(10);

            MockGoodsInStock.Verify(m => m.Remove(goodsInStock), Times.Once());
        }

        [Test]
        public void OrderDelete_TryDeleteOneOrderById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10,
                Id = 10
            };
            var order = new Order()
            {
                Goods = goods,
                Count = 10,
                GoodsId = 10,
                Id = 10,
                OrderStatus = DAL.Statuses.OrderStatus.AwaitingShipment
            };
            MockOrder.Setup(m => m.Find(10)).Returns(order);
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);

            UoW.Orders.Delete(10);

            MockOrder.Verify(m => m.Remove(order), Times.Once());
        }

        [Test]
        public void UserDelete_TryDeleteOneUserById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var user = new User()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 12345,
                Id=10, 
                UserStatus=DAL.Statuses.UserStatus.Customer
            };
            MockUser.Setup(m => m.Find(10)).Returns(user);
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);

            UoW.Users.Delete(10);

            MockUser.Verify(m => m.Remove(user), Times.Once());
        }

        [Test]
        public void OrderListDelete_TryDeleteOneOrderListById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var user = new User()
            {
                FirstName = "Any",
                LastName = "Any",
                PhoneNumber = 12345,
                Id = 10,
                UserStatus = DAL.Statuses.UserStatus.Customer
            };
            var orderList = new OrderList()
            {
                User = user,
                Id=10, 
                UserId=10
            };
            MockOrderList.Setup(m => m.Find(10)).Returns(orderList);
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);

            UoW.OrderLists.Delete(10);

            MockOrderList.Verify(m => m.Remove(orderList), Times.Once());
        }

        [Test]
        public void QueueForPurchaseDelete_TryDeleteOneQueueForPurchaseById_MethodDeleteFromMockOfApplicationContextCalledOnce()
        {
            var goods = new Goods()
            {
                Name = "Any",
                Price = 10,
                Id = 10,
                GoodsStatus = DAL.Statuses.GoodsStatus.NotAvailable
            };
            var queueForPurchase = new QueueForPurchase()
            {
                Goods=goods,
                Count=10,
                Id=10,
                GoodsId=10,
                Priority=DAL.Priority.GoodsInQueuePriority.Medium
            };
            MockQueueForPurchase.Setup(m => m.Find(10)).Returns(queueForPurchase);
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);

            UoW.QueueForPurchase.Delete(10);

            MockQueueForPurchase.Verify(m => m.Remove(queueForPurchase), Times.Once());
        }

        [Test]
        public void GoodsGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Price=1 },
                new Goods() { Name="Any", Price=2 },
                new Goods() { Name="Any", Price=3 }
            }.AsQueryable();
            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.Goods.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GoodsInStockGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<GoodsInStock>
            {
                new GoodsInStock(){ Goods=new Goods() { Name="Any", Price=1 }, Count=1 },
                new GoodsInStock(){ Goods=new Goods() { Name="Any", Price=2 }, Count=2 },
                new GoodsInStock(){ Goods=new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockGoodsInStock.As<IQueryable<GoodsInStock>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.GoodsInStock.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }

        [Test]
        public void OrderGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<Order>
            {
                new Order(){ Goods=new Goods() { Name="Any", Price=1 }, Count=1 },
                new Order(){ Goods=new Goods() { Name="Any", Price=2 }, Count=2 },
                new Order(){ Goods=new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);

            var orders = UoW.Orders.GetAll().ToList();

            orders.Count.Should().Be(3);
            orders.Should().BeEquivalentTo(data);
        }

        [Test]
        public void UsersGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<User>
            {
                new User() { FirstName="Any", LastName="Any", Id=1 },
                new User() { FirstName="Any", LastName="Any", Id=2 },
                new User() { FirstName="Any", LastName="Any", Id=3 }
            }.AsQueryable();
            MockUser.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.Users.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }
        
        [Test]
        public void OrderListGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<OrderList>
            {
                new OrderList(){ User=new User() { FirstName="Any", LastName="Any", Id=1 }, Id=1 },
                new OrderList(){ User=new User() { FirstName="Any", LastName="Any", Id=2 }, Id=2 },
                new OrderList(){ User=new User() { FirstName="Any", LastName="Any", Id=3 }, Id=3 }
            }.AsQueryable();
            MockOrderList.As<IQueryable<OrderList>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.OrderLists.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }

        [Test]
        public void QueueForPurchaseGetAll_TryModelGetingAllElementFromDbWithUsingList_GetListWithThirdElement()
        {
            var data = new List<QueueForPurchase>
            {
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=1 }, Count=1 },
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=2 }, Count=2 },
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            MockQueueForPurchase.As<IQueryable<QueueForPurchase>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);

            var categories = UoW.QueueForPurchase.GetAll().ToList();

            categories.Count.Should().Be(3);
            categories.Should().BeEquivalentTo(data);
        }

        [Test]
        public void GoodsFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<Goods, bool>> predicate = (Goods g) => g.Price > 1;
            var data = new List<Goods>
            {
                new Goods() { Name="Any", Price=1 },
                new Goods() { Name="Any", Price=2 },
                new Goods() { Name="Any", Price=3 }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockGoods.As<IQueryable<Goods>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Goods>()).Returns(MockGoods.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.Goods.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void GoodsInStockFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<GoodsInStock, bool>> predicate = (GoodsInStock g) => g.Count == 20;
            var data = new List<GoodsInStock>
            {
                new GoodsInStock(){ Goods = new Goods() { Name = "Any", Price = 1 }, Count = 10, },
                new GoodsInStock(){ Goods = new Goods() { Name = "Any", Price = 2 }, Count = 20, },
                new GoodsInStock(){ Goods = new Goods() { Name = "Any", Price = 3 }, Count = 30, }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockGoodsInStock.As<IQueryable<GoodsInStock>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<GoodsInStock>()).Returns(MockGoodsInStock.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.GoodsInStock.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void OrderFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<Order, bool>> predicate = (Order g) => g.Count >= 20;
            var data = new List<Order>
            {
                new Order(){ Goods = new Goods() { Name = "Any", Price = 1 }, Count = 10, },
                new Order(){ Goods = new Goods() { Name = "Any", Price = 2 }, Count = 20, },
                new Order(){ Goods = new Goods() { Name = "Any", Price = 3 }, Count = 30, }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockOrder.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<Order>()).Returns(MockOrder.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.Orders.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UserFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<User, bool>> predicate = (User g) => g.PhoneNumber == 1;
            var data = new List<User>
            {
                new User() { FirstName="Any", LastName="Any", PhoneNumber=1 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=2 },
                new User() { FirstName="Any", LastName="Any", PhoneNumber=3 }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockUser.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<User>()).Returns(MockUser.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.Users.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void OrderListFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<OrderList, bool>> predicate = (OrderList g) => g.Id == 3;
            var data = new List<OrderList>
            {
                new OrderList(){ User=  new User() { FirstName="Any", LastName="Any", PhoneNumber=1 }, Id = 1 },
                new OrderList(){ User=  new User() { FirstName="Any", LastName="Any", PhoneNumber=1 }, Id = 2 },
                new OrderList(){ User=  new User() { FirstName="Any", LastName="Any", PhoneNumber=1 }, Id = 3 }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockOrderList.As<IQueryable<OrderList>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<OrderList>()).Returns(MockOrderList.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.OrderLists.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void QueueForPurchaseFind_TryModelGetingSomeElementByPredicateFromDbWithUsingList_GetListWithThirdElement()
        {
            Expression<Func<QueueForPurchase, bool>> predicate = (QueueForPurchase g) => g.Id == 3;
            var data = new List<QueueForPurchase>
            {
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=1 }, Count=1 },
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=2 }, Count=2 },
                new QueueForPurchase(){ Goods=new Goods() { Name="Any", Price=3 }, Count=3 }
            }.AsQueryable();
            var expected = data.Where(predicate).ToList();

            MockQueueForPurchase.As<IQueryable<QueueForPurchase>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
            MockContext.Setup(c => c.Set<QueueForPurchase>()).Returns(MockQueueForPurchase.Object);
            UoW = new(MockContext.Object);

            var actual = UoW.QueueForPurchase.Find(predicate).ToList();

            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void Save_InvodeMethodSaveChanges_MethodSaveChangesInvokedOnce()
        {
            UoW = new(MockContext.Object);

            UoW.Save();

            MockContext.Verify(m => m.SaveChanges(), Times.Once());
        }
    }
}