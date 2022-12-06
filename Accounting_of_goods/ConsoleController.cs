using static System.Console;
using BLL.Entities;

namespace Accounting_of_goods
{
    public static class ConsoleController
    {
        public static int GetIngex(int MaxValue, string ToUser)
        {
            int result = 0;
            bool readed = false;
            while (!readed)
            {
                Write(ToUser);
                readed = Int32.TryParse(ReadLine(), out result);
                if (readed && (result < 1 || result > MaxValue))
                {
                    readed = false;
                    Write("Incorect input\n");
                }
            }
            result -= 1;
            return result;
        }
        public static int GetNumber(string ToUser)
        {
            int result = 0;
            bool readed = false;
            while (!readed)
            {
                Write(ToUser);
                readed = Int32.TryParse(ReadLine(), out result);
                if (!readed)
                {
                    Write("Incorect input\n");
                }
            }
            return result;
        }
        public static string GetStrint(string ToUser)
        {
            Write(ToUser);
            var result = ReadLine();
            return result;
        }
        public static void ShowGoods(List<GoodsBLL> goods)
        {
            Write("\t\tName\t\tCount\t\tPrice\t\tCategory\n");
            int i = 1;
            foreach (var item in goods)
            {
                var count = item.Count;
                if (count < 0)
                {
                    count = 0;
                }
                Write((i++) + "\t\t" +
                    item.Name + "\t\t" +
                    count + "\t\t" +
                    item.Priсe + "\t\t"+
                    item.CategoryBLL?.Name + "\t\t\n");
            }
        }
        public static void ShowCategory(List<CategoryBLL> categories)
        {
            Write("\tName\n");
            int i = 1;
            foreach (var item in categories)
            {
                Write((i++) + "\t" + item.Name + "\n");
            }
        }
        public static void Write(string ToUser)
        {
            Console.Write(ToUser);
        }

        public static bool YNquestion(string question)
        {
            Write(question+" (y/n)\n");
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;
            else
                return false;
        }

        public static void Clear()
        {
            Console.Clear();
        }
        public static string ReadLine()
        {
            return Console.ReadLine();
        }

        public static ConsoleKey UserWish(string WishList)
        {
            Write(WishList);
            var result = ReadKey().Key;
            return result;
        }
    }
}
