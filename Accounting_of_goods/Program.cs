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
        static bool BaseEvents()
        {
            var UoW = new UnitOfWork();
            var Goods = new GetGoods(UoW);
            var Categories = new GetCategory(UoW);
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
                    var input = Parsing(categories.Count, "Select category (Enter number): ");
                    goods = Goods.GetAllFollowing(categories[input].Id);
                    Clear();
                    ShowGoods(goods);
                    break;

                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    goods = Goods.GetAll();
                    Clear();
                    ShowGoods(goods);
                    input = Parsing(goods.Count, "Select goods (Enter number): ");
                    var CurrentGoods = Goods.GetCurrent(goods[input].Id);
                    Clear();
                    WriteLine("Name\t\tCount\t\tPrice\t\tCategory");
                    WriteLine(CurrentGoods.Name + "\t" + CurrentGoods.Count + "\t" + CurrentGoods.Priсe + "\t" + CurrentGoods.Category?.Name);
                    WriteLine();
                    if (StartEventWithEntity("Do you want do something with this goods?(y/n)"))
                    {
                        GoodsEvents(CurrentGoods);
                    }
                    break;
                case ConsoleKey.D6:
                case ConsoleKey.NumPad6:
                    Category NewCategory = CreatLocelCategory();
                    var categoryCreater = new CategoryCreater(UoW);
                    categoryCreater.Creat(NewCategory);
                    break;

                case ConsoleKey.D7:
                case ConsoleKey.NumPad7:
                    var NewGoods = (CteatLocalCategory(Categories));
                    var goodsCreater = new GoodsCreater(UoW);
                    goodsCreater.Creat(NewGoods);
                    break;
                case ConsoleKey.D8:
                case ConsoleKey.NumPad8:
                    categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    input = Parsing(categories.Count, "Select goods (Enter number): ");
                    var CurrentCategory = Categories.GetCurrent(categories[input].Id);
                    Clear();
                    WriteLine("Name");
                    WriteLine(CurrentCategory.Name);
                    WriteLine();
                    if (StartEventWithEntity("Do you want do something with this Category?(y/n)"))
                    {
                        CategoryEvents(CurrentCategory);
                    }
                    break;
            }
            return true;
        }
        static void ShowGoods(List<Goods> goods)
        {
            WriteLine("\t\tName\t\tCount\t\tPrice\t\tCategory");
            int i = 1;
            foreach (var item in goods)
            {
                WriteLine((i++) + "\t\t" + item.Name + "\t\t" + item.Count + "\t\t" + item.Priсe + "\t\t" + item.Category?.Name);
            }
        }
        static void ShowCategory(List<Category> categories)
        {
            WriteLine("\tName");
            int i = 1;
            foreach (var item in categories)
            {
                WriteLine((i++) + "\t" + item.Name);
            }
        }
        static int Parsing(int MaxValue, string InputString)
        {
            int result = 0;
            bool readed = false;
            while (!readed)
            {
                Write(InputString);
                readed = Int32.TryParse(ReadLine(), out result);
                if (readed && (result < 1 || result > MaxValue))
                {
                    readed = false;
                    WriteLine("Incorect input");
                }
            }
            result -= 1;
            return result;
        }
        static int Parsing(string InputString)
        {

            int result = 0;
            bool readed = false;
            while (!readed)
            {
                Write(InputString);
                readed = Int32.TryParse(ReadLine(), out result);
                if (!readed)
                {
                    WriteLine("Incorect input");
                }
            }
            return result;
        }
        static Goods CteatLocalCategory(GetCategory Categories)
        {
            Clear();
            Write("Input name for goods: ");
            string goodsName = ReadLine();
            Clear();
            int goodsPrice = Parsing("Input price for new goods: ");
            Clear();
            int goodsCount = Parsing("Input count for new goods: ");
            var categories = Categories.GetAll();
            Clear();
            ShowCategory(categories);
            var input = Parsing(categories.Count, "Select category (Enter number): ");
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
            Write("Input name for  category: ");
            string categoryName = ReadLine();
            Category NewCategory = new Category() { Name = categoryName };
            return NewCategory;
        }
        static bool StartEventWithEntity(string inputString)
        {
            WriteLine(inputString);
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;
            else
                return false;
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

        static void GoodsEvents(Goods goods)
        {
            var Event = UserWishGoods();
            var UoW = new UnitOfWork();
            var Categories = new GetCategory(UoW);

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
                    goods.Priсe = Parsing("Input price for new goods: ");
                    Clear();
                    goods.Count = Parsing("Input count for new goods: ");
                    var categories = Categories.GetAll();
                    Clear();
                    ShowCategory(categories);
                    var input = Parsing(categories.Count, "Select category (Enter number): ");
                    goods.Category = categories[input];
                    var Updater = new GoodsUpDater(UoW);
                    Updater.UpDate(goods);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    var Deleter = new GoodsDeleter(UoW);
                    Deleter.Delete(goods);
                    break;

                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    var Seller = new GoodsSeller(UoW);
                    int Count = Parsing("Enter count you wish: ");
                    Seller.Sell(Count, goods.Id);
                    break;
            }
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

        static void CategoryEvents(Category category)
        {
            var Event = UserWishCategory();
            var UoW = new UnitOfWork();

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

                    var Updater = new CategoryUpDater(UoW);
                    Updater.UpDate(category);
                    break;

                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    var Deleter = new CategoryDeleter(UoW);
                    Deleter.Delete(category);
                    break;
            }
        }
        
    }
}