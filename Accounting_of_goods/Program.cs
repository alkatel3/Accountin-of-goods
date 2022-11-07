using UoW;
using DAL.Entities;
using static System.Console;
using BLL;
using DAL;

namespace Accounting_of_goods
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (BaseEvents())
            {
                WriteLine();
            }
        }
        static ConsoleKey UserWish()
        {
            WriteLine("What do you wish? (Enter number)\n" +
                "1. Get all goods\n" +
                "2. Get all categories\n" +
                "3. Get goods in stock\n" +
                "4. Get goods in some category\n" +
                "5. Select goods\n" +
                "6. Creat new category\n" +
                "7. Creat new goods\n" +
                "8. Select category\n" +
                "0. Exit");
            var result = ReadKey().Key;
            return result;
        }
        static ConsoleKey UserWishGoods()
        {
            WriteLine("What do you wish? (Enter number)\n" +
                "1. Change goods\n" +
                "2. Delete goods\n" +
                "3. Sell goods\n" +
                "0. Exit");
            var result = ReadKey().Key;
            return result;
        }
        static ConsoleKey UserWishCategory()
        {
            WriteLine("What do you wish? (Enter number)\n" +
                "1. Change category\n" +
                "2. Delete category\n" +
                "0. Exit");
            var result = ReadKey().Key;
            return result;
        }

        static bool BaseEvents()
        {
            var UoW = new UnitOfWork();
            var Goods = new GoodsController(UoW);
            var Categories = new CategoryController(UoW);
            var result = UserWish();

            switch (result)
            {
                case ConsoleKey.D0:
                case ConsoleKey.NumPad0:
                    return false;

                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    var goods = Goods.GetAll();
                    Clear();
                    ConsoleController.ShowGoods(goods);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    var categories = Categories.GetAll();
                    Clear();
                    ConsoleController.ShowCategory(categories);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    goods = Goods.GetInStock();
                    Clear();
                    ConsoleController.ShowGoods(goods);
                    break;

                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    categories = Categories.GetAll();
                    Clear();
                    ConsoleController.ShowCategory(categories);
                    var input = ConsoleController.GetIngex(categories.Count, "Select category (Enter number): ");
                    goods = Goods.GetAllFollowing(categories[input].Id);
                    Clear();
                    ConsoleController.ShowGoods(goods);
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    goods = Goods.GetAll();
                    Clear();
                    ConsoleController.ShowGoods(goods);
                    input = ConsoleController.GetIngex(goods.Count, "Select goods (Enter number): ");
                    var CurrentGoods = Goods.GetCurrent(goods[input].Id);
                    Clear();
                    WriteLine("Name\t\tCount\t\tPrice\t\tCategory");
                    WriteLine(CurrentGoods.Name + "\t" + CurrentGoods.Count + "\t" + CurrentGoods.Priсe + "\t" + CurrentGoods.Category?.Name);
                    WriteLine();
                    if (ConsoleController.YNquestion("Do you want do something with this goods?"))
                    {
                        GoodsEvents(CurrentGoods);
                    }
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    Category NewCategory = CreatLocelCategory();
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
                    ConsoleController.ShowCategory(categories);
                    input = ConsoleController.GetIngex(categories.Count, "Select category (Enter number): ");
                    var CurrentCategory = Categories.GetCurrent(categories[input].Id);
                    Clear();
                    WriteLine("Name");
                    WriteLine(CurrentCategory.Name);
                    WriteLine();
                    if (ConsoleController.YNquestion("Do you want do something with this Category?"))
                    {
                        CategoryEvents(CurrentCategory);
                    }
                    break;
            }
            return true;
        }
        static void GoodsEvents(Goods goods)
        {
            var Event = UserWishGoods();
            var UoW = new UnitOfWork();
            var Categories = new CategoryController(UoW);
            var Goods = new GoodsController(UoW);

            switch (Event)
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
                    goods.Priсe = ConsoleController.GetNumber("Input price for new goods: ");
                    Clear();
                    goods.Count = ConsoleController.GetNumber("Input count for new goods: ");
                    var categories = Categories.GetAll();
                    Clear();
                    ConsoleController.ShowCategory(categories);
                    var input = ConsoleController.GetIngex(categories.Count, "Select category (Enter number): ");
                    goods.Category = categories[input];
                    Goods.UpDate(goods);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    if (ConsoleController.YNquestion("Are you sure?"))
                        Goods.Delete(goods);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    var Seller = new GoodsSeller(UoW);
                    int Count = ConsoleController.GetNumber("Enter count you wish: ");
                    Seller.Sell(Count, goods.Id);
                    break;
            }
        }
        static void CategoryEvents(Category category)
        {
            var Event = UserWishCategory();
            var UoW = new UnitOfWork();
            var Categories = new CategoryController(UoW);

            switch (Event)
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

                    if (ConsoleController.YNquestion("If Category contais goods it will be delete to0. Are you sure?"))
                        Categories.Delete(category);
                    break;
            }
        }

        static Goods CteatLocalGoods(CategoryController Categories)
        {
            Clear();
            string goodsName = ConsoleController.GetStrint("Input name for goods: ");
            Clear();
            int goodsPrice = ConsoleController.GetNumber("Input price for new goods: ");
            Clear();
            int goodsCount = ConsoleController.GetNumber("Input count for new goods: ");
            var categories = Categories.GetAll();
            Clear();
            ConsoleController.ShowCategory(categories);
            var input = ConsoleController.GetIngex(categories.Count, "Select category (Enter number): ");
            Category goodsCategory = categories[input];
            Goods NewGoods = new Goods()
            {
                Name = goodsName,
                Priсe = goodsPrice,
                Count = goodsCount,
                Category = goodsCategory
            };
            return NewGoods;
        }
        static Category CreatLocelCategory()
        {
            Clear();
            string categoryName = ConsoleController.GetStrint("Input name for  category: ");
            Category NewCategory = new Category() { Name = categoryName };
            return NewCategory;
        }

        


      


        
    }
}