using BLL.DTO;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Accounting_of_goods.ConsoleController;

namespace Accounting_of_goods
{
    public class SellerMenu
    {
        private ISellerServise sellerServise;
        private UserDTO seller = null!;

        public SellerMenu(ISellerServise sellerServise)
        {
            this.sellerServise = sellerServise;
            this.sellerServise.CantSaveChanges += CantSaveChanges;
        }

        public UiCommandType StartSellerMenu()
        {
            while (this.seller == null)
            {
                var result = YNquestion("Have you got accout?");
                if (result)
                {
                    this.seller = Verify.SigningIn(sellerServise);
                }
                else
                {
                    this.seller = Verify.SigningUp(sellerServise);
                }
            }
            return UiCommandType.SellerActionsList;
        }

        public UiCommandType SellerActionsList()
        {
            var keyValuePairs = new Dictionary<ConsoleKey, UiCommandType>
            {
                [ConsoleKey.D1] = UiCommandType.CreatGoods,
                [ConsoleKey.D2] = UiCommandType.AddGoods,
                [ConsoleKey.D3] = UiCommandType.ShowAllGoods,
                [ConsoleKey.D4] = UiCommandType.ShowGoodsInStock,
                [ConsoleKey.D5] = UiCommandType.ShowOrders,
                [ConsoleKey.D6] = UiCommandType.CompleteOrder,
                [ConsoleKey.D7] = UiCommandType.ShowQueueForPurchase,

                [ConsoleKey.D0] = UiCommandType.Exit
            };


            ConsoleKey key = ConsoleKey.Z;
            while (!keyValuePairs.ContainsKey(key))
            {
                int i = 1;
                key = UserWish($"\n(input number)\n" +
                                $"{i++}. Creat new goods\n" +
                                $"{i++}. Add goods\n" +
                                $"{i++}. Show all goods\n" +
                                $"{i++}. Show goods in stock\n" +
                                $"{i++}. Show outstanding orders\n" +
                                $"{i++}. Complete the order\n" +
                                $"{i++}. Show queue For purchase\n" +
                                $"0. Exit\n");
            }
            return keyValuePairs[key];
        }

        public UiCommandType CreatGoods()
        {
            var goodsName = GetStrint("Enter the name of the goods: ");
            var goodsPrice = GetPrice("Enter the price of the goods: ");
            var goods = new GoodsDTO()
            {
                Name = goodsName,
                Priсe = goodsPrice
            };
            sellerServise.CreateGoods(goods);
            Write($"{goodsName} with price {goodsPrice} created");
            return UiCommandType.SellerActionsList;
        }

        public UiCommandType AddGoods()
        {
            var goodsList = sellerServise.GetAllGoods();
            ShowGoods(goodsList, true);
            var index = GetIngex(goodsList.Count, "To select goods enter number of goods: ");
            var goods = goodsList[index];
            var count = GetNumber("How many goods do you want add?\n");
            sellerServise.AddGoods(goods, count);

            return UiCommandType.SellerActionsList;
        }

        public UiCommandType ShowAllGoods()
        {
            var goodsList = sellerServise.GetAllGoods();
            ShowGoods(goodsList, true);
            var index = GetIngex(goodsList.Count, "To select goods enter number of goods: ");
            var goods = goodsList[index];
            //TODO Show Goods
            var result = YNquestion("Do you want change this goods?");
            if (result)
            {
                goods.Name = GetStrint($"Input new name for goods \"{goods.Name}\": ");
                goods.Priсe = GetPrice($"Input new price for goods \"{goods.Name}\": ");
                sellerServise.UpdateGoods(goods);
                Write($"Goods \"{goods.Name}\" Updated.\n");
            }
            return UiCommandType.SellerActionsList;
        }

        public UiCommandType ShowGoodsInStock()
        {
            var goodsList = sellerServise.GetGoodsInStocks();
            ShowGoods(goodsList);
            var index = GetIngex(goodsList.Count, "To select goods enter number of goods: ");
            var goods = goodsList[index];
            var result = YNquestion("Do you want change this goods?");
            if (result)
            {
                goods.Goods.Name = GetStrint($"Input new name for goods \"{goods.Goods.Name}\": ");
                goods.Goods.Priсe = GetPrice($"Input new price for goods \"{goods.Goods.Name}\": ");
                goods.Count = GetNumber($"Input new count for goods \"{goods.Goods.Name}\": ");
                sellerServise.UpdateGoods(goods);
                Write($"Goods \"{goods.Goods.Name}\" Updated.\n");
            }
            return UiCommandType.SellerActionsList;
        }

        public UiCommandType ShowOrders()
        {
            var orders = sellerServise.GetOrders();

            ShowOrderList(orders);
            ReadLine();
            return UiCommandType.SellerActionsList;
        }

        public UiCommandType CompleteOrder()
        {
            var orders = sellerServise.GetOrders();

            ShowOrderList(orders);
            var index = GetIngex(orders.Count, "To select order enter number of goods: ");
            var order = orders[index];
            sellerServise.ProcessOrder(order);
            return UiCommandType.SellerActionsList;

        }

        public UiCommandType ShowQueueForPurchase()
        {
            var queueForPurchase = sellerServise.GetQueueForPurchase();
            ShowGoodsInQueue(queueForPurchase);
            var index = GetIngex(queueForPurchase.Count, "To select goods in queue enter number of goods: ");
            var goods = queueForPurchase[index];
            var result = YNquestion("Do you want to bring these goods?");
            if (result)
            {
                sellerServise.BringGoodsFromQueueForPurchase(goods);
            }
            return UiCommandType.SellerActionsList;
        }

        private void CantSaveChanges(object sender, PropertyValues e)
        {
            Write("Action failed, try again later");
        }
    }
}
