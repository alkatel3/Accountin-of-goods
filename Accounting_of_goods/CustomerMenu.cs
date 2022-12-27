using BLL.DTO;
using BLL.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static Accounting_of_goods.ConsoleController;

namespace Accounting_of_goods
{
    public class CustomerMenu
    {
        private readonly ICustomerService customerService;
        private GoodsDTO goods=null!;
        private UserDTO client=null!;

        public CustomerMenu(ICustomerService customerService)
        {
            this.customerService = customerService;
            this.customerService.CantSaveChanges += CantSaveChanges;
        }

        public UiCommandType StartCustomerMenu()
        {
            var keyValuePairs = new Dictionary<ConsoleKey, UiCommandType> {
                [ConsoleKey.D1] = UiCommandType.ShowClientOrderList,
                [ConsoleKey.D2] = UiCommandType.PickUpGoods,
                [ConsoleKey.D0] = UiCommandType.Exit
                };

            ConsoleKey key = ConsoleKey.Z;
            while (!keyValuePairs.ContainsKey(key))
            {
                int i = 1;
                key = UserWish($"\n(input number)\n" +
                                $"{i++}. Get client order list\n" +
                                $"{i++}. Pick up goods\n" +
                                $"0. Exit\n");
            }
            return keyValuePairs[key];
        }

        public UiCommandType ShowClientOrderList()
        {
            if (this.client == null)
            {
                var phone = (int)GetNumber("\nInput your phone(Enter numder)\n> ");
                this.client = customerService.GetAccount(phone);            }
            if (this.client == null)
            {
                Write("You account didn't find\n");
            }
            else
            {
                ShowOrderList(this.client.OrderList?.Orders);
            }
            Write("Press Enter to continue");
            ReadLine();
            Clear();
            return UiCommandType.StartCustomerMenu;
        }

        public UiCommandType PickUpGoods()
        {
            var goodsList = customerService.GetAllGoods();
            ShowGoods(goodsList, false);
            var index = GetIngex(goodsList.Count, "To select goods enter number of goods: ");
            var goods = goodsList[index];
            var result = YNquestion("Do you want buy this goods?");
            if (result)
            {
                this.goods = goods;
                return UiCommandType.VerifyMenu;
            }
            return UiCommandType.PickUpGoods;

        }

        public UiCommandType BuyGoods()
        {
            var count = GetNumber("How many goods do you wish: ");
            customerService.CreatOrder(client, this.goods, count);
            this.client = customerService.GetAccount(client.PhoneNumber);
            ReadLine();
            return UiCommandType.StartCustomerMenu;
        }

        public UiCommandType VerifyMenu()
        {
            while (this.client == null)
            {
                var result = YNquestion("Have you got accout?");
                if (result)
                {
                    this.client = Verify.SigningIn(customerService);
                }
                else
                {
                    this.client = Verify.SigningUp(customerService);
                }
            }
            return UiCommandType.BuyGoodsMenu;
        }

        private void CantSaveChanges(object sender, PropertyValues e)
        {
            Write("Action failed, try again later");
        }
    }
}
