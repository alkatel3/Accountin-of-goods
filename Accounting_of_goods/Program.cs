using static Accounting_of_goods.ConsoleController;
using BLL;
using DAL.Repositories;
using DAL.EF;
using BLL.Entities;

namespace Accounting_of_goods
{

    internal class Program
    {
        static EFUnitOfWork UoW = new EFUnitOfWork(new ApplicationContext());
        static GoodsController Goods = new GoodsController(UoW);
        static CategoryController Categories = new CategoryController(UoW);

        static void Main(string[] args)
        {
            while (BaseEvents())
            {
                Write("\n");
            }
        }

        static bool BaseEvents()
        {
            var result=UserWish("What do you wish? (Enter number)\n" +
                "1. Get all goods\n" +
                "2. Get all categories\n" +
                "3. Get goods in stock\n" +
                "4. Get goods in some category\n" +
                "5. Select goods\n" +
                "6. Creat new category\n" +
                "7. Creat new goods\n" +
                "8. Select category\n" +
                "0. Exit\n");

            switch (result)
            {
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    Goods.Dispose();
                    Categories.Dispose();
                    return false;

                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    var goods = Goods.GetAll();
                    Clear();
                    ShowGoods(goods);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    var categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    goods = Goods.GetInStock();
                    Clear();
                    ShowGoods(goods);
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    var input = GetIngex(categories.Count, "Select category (Enter number): ");
                    goods = Goods.GetAllFollowing(categories[input].Id);
                    Clear();
                    ShowGoods(goods);
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    goods = Goods.GetAll();
                    Clear();
                    ShowGoods(goods);
                    input = GetIngex(goods.Count, "Select goods (Enter number): ");
                    var CurrentGoods = Goods.GetCurrent(goods[input].Id);
                    Clear();
                    Write("Name\t\tCount\t\tPrice\t\tCategory\n");
                    Write(CurrentGoods.Name + "\t\t" + CurrentGoods.Count + "\t\t" + CurrentGoods.Priсe + "\t\t" + CurrentGoods.CategoryBLL?.Name+"\n");
                    Write("\n");
                    if (YNquestion("Do you want do something with this goods?"))
                    {
                        GoodsEvents(CurrentGoods);
                    }
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    CategoryBLL NewCategory = CreatLocalCategory();
                    Categories.Creat(NewCategory);
                    break;

                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    var NewGoods = (CteatLocalGoods(Categories));
                    Goods.Creat(NewGoods);
                    break;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    input = GetIngex(categories.Count, "Select category (Enter number): ");
                    var CurrentCategory = Categories.GetCurrent(categories[input].Id);
                    Clear();
                    Write("Name\n");
                    Write(CurrentCategory.Name+"\n");
                    Write("\n");
                    if (YNquestion("Do you want do something with this Category?"))
                    {
                        CategoryEvents(CurrentCategory);
                    }
                    break;
            }
            return true;
        }
        static void GoodsEvents(GoodsBLL goods)
        {
            var result = UserWish("What do you wish? (Enter number)\n" +
                "1. Change goods\n" +
                "2. Delete goods\n" +
                "3. Sell goods\n" +
                "0. Exit\n");

            switch (result)
            {
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    return;

                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Clear();
                    Write("Input name for goods: ");
                    goods.Name = ReadLine();
                    Clear();
                    goods.Priсe = GetNumber("Input price for new goods: ");
                    Clear();
                    goods.Count = GetNumber("Input count for new goods: ");
                    var categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    var input = GetIngex(categories.Count, "Select category (Enter number): ");
                    goods.CategoryBLL = categories[input];
                    Goods.UpDate(goods);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    if (YNquestion("Are you sure?"))
                        Goods.Delete(goods);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    var Seller = new GoodsSeller(Goods);
                    int Count = GetNumber("Enter count you wish: ");
                    Seller.Sell(Count, goods);
                    break;
            }
        }
        static void CategoryEvents(CategoryBLL category)
        {
            var result = UserWish("What do you wish? (Enter number)\n" +
                "1. Change category\n" +
                "2. Delete category\n" +
                "0. Exit\n");

            switch (result)
            {
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    return;

                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Clear();
                    Write("Input name for category: ");
                    category.Name = ReadLine();

                    Categories.UpDate(category);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:

                    if (YNquestion("If Category contais goods it will be delete to0. Are you sure?"))
                        Categories.Delete(category);
                    break;
            }
        }

        static GoodsBLL CteatLocalGoods(CategoryController Categories)
        {
            Clear();
            string goodsName = GetStrint("Input name for goods: ");
            Clear();
            int goodsPrice = GetNumber("Input price for new goods: ");
            Clear();
            int goodsCount = GetNumber("Input count for new goods: ");
            var categories = Categories.GetAll();
            Clear();
            ShowCategory(categories);
            var input = GetIngex(categories.Count, "Select category (Enter number): ");
            CategoryBLL goodsCategory = categories[input];
            GoodsBLL NewGoods = new GoodsBLL()
            {
                Name = goodsName,
                Priсe = goodsPrice,
                Count = goodsCount,
                CategoryBLL = goodsCategory
            };

            return NewGoods;
        }
        static CategoryBLL CreatLocalCategory()
        {
            Clear();
            string categoryName = GetStrint("Input name for  category: ");
            CategoryBLL NewCategory = new CategoryBLL() { Name = categoryName };
            return NewCategory;
        }
    }
}