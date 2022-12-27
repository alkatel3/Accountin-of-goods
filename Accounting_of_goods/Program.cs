using BLL.DTO;
using BLL.Interfaces;
using BLL.Servises;
using DAL.EF;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Accounting_of_goods.ConsoleController;

namespace Accounting_of_goods
{
    public enum UiCommandType
    {
        StartMenu,

        CustomerMenu,
        StartCustomerMenu,
        PickUpGoods,
        ShowClientOrderList,
        BuyGoodsMenu,

        SellerMenu,
        StartSellerMenu,
        SellerActionsList,
        ShowQueueForPurchase,
        CreatGoods,
        AddGoods,
        ShowAllGoods,
        ShowGoodsInStock,
        ShowOrders,
        CompleteOrder,

        VerifyMenu,
        Exit,
    }

    public delegate UiCommandType CommandType();

    internal class Program
    {
        static readonly UnitOfWork UoW = new(new ApplicationContext());
        static readonly ICustomerService customerService = new CustomerServise(UoW);
        static readonly ISellerServise sellerServise = new SellerServise(UoW);

        static Dictionary<UiCommandType, CommandType> UserCommands=null!;
        static CommandType Action=null!;

        static void Main(string[] args)
        {
            Action = StartMenu;
            while (true)
            {
                var nextMenu=Action.Invoke();
                if (nextMenu == UiCommandType.Exit)
                    break;
                InitializeStartMenu();
                Action = UserCommands[nextMenu];
            }
        }
        
        public static void InitializeStartMenu()
        {
            UserCommands = new Dictionary<UiCommandType, CommandType>
            {
                [UiCommandType.CustomerMenu] = CustomerMenu,
                [UiCommandType.SellerMenu] = SellerMenu,
                [UiCommandType.StartMenu] = StartMenu
            };
        }
        public static UiCommandType StartMenu()
        {
            var keyValuePairs = new Dictionary<ConsoleKey, UiCommandType>
            {
                [ConsoleKey.D1] = UiCommandType.CustomerMenu,
                [ConsoleKey.D2] = UiCommandType.SellerMenu,
                [ConsoleKey.D0] = UiCommandType.Exit
            };


            ConsoleKey key = ConsoleKey.Z;
            while (!keyValuePairs.ContainsKey(key))
            {
                int i = 1;
                key = UserWish($"What is you role?(input number)\n" +
                $"{i++}. Customer\n" +
                $"{i++}. Seller\n" +
                $"0. Exit\n");
            }
            return keyValuePairs[key];

        }

        public static void InitializeCustomerMenu()
        {
            var customerMenu = new CustomerMenu(customerService);
            UserCommands = new Dictionary<UiCommandType, CommandType>
            {
                [UiCommandType.StartCustomerMenu] = customerMenu.StartCustomerMenu,
                [UiCommandType.ShowClientOrderList] = customerMenu.ShowClientOrderList,
                [UiCommandType.PickUpGoods] = customerMenu.PickUpGoods,
                [UiCommandType.BuyGoodsMenu] = customerMenu.BuyGoods,
                [UiCommandType.VerifyMenu] = customerMenu.VerifyMenu
            };
        }
        public static UiCommandType CustomerMenu()
        {
            InitializeCustomerMenu();
            Action = UserCommands[UiCommandType.StartCustomerMenu];
            while (true)
            {
                var nextMenu=Action.Invoke();
                if (nextMenu == UiCommandType.Exit)
                    break;

                Action = UserCommands[nextMenu];
            }
            return UiCommandType.StartMenu;
        }

        public static void InitializeSellerMenu()
        {
            var sellerMenu = new SellerMenu(sellerServise);
            UserCommands = new Dictionary<UiCommandType, CommandType>
            {
                [UiCommandType.StartSellerMenu] = sellerMenu.StartSellerMenu,
                [UiCommandType.SellerActionsList] = sellerMenu.SellerActionsList,
                [UiCommandType.CreatGoods] = sellerMenu.CreatGoods,
                [UiCommandType.AddGoods] = sellerMenu.AddGoods,
                [UiCommandType.ShowAllGoods] = sellerMenu.ShowAllGoods,
                [UiCommandType.ShowGoodsInStock] = sellerMenu.ShowGoodsInStock,
                [UiCommandType.ShowOrders] = sellerMenu.ShowOrders,
                [UiCommandType.CompleteOrder] = sellerMenu.CompleteOrder,
                [UiCommandType.ShowQueueForPurchase] = sellerMenu.ShowQueueForPurchase
            };
        }   
        static UiCommandType SellerMenu()
        {
            InitializeSellerMenu();
            Action = UserCommands[UiCommandType.StartSellerMenu];
            while (true)
            {
                var nextMenu = Action.Invoke();
                if (nextMenu == UiCommandType.Exit)
                    break;

                Action = UserCommands[nextMenu];
            }
            return UiCommandType.StartMenu;
        }

    }
}