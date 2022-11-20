using static System.Console;
using BLL.Entities;

namespace Accounting_of_goods
{
    public class ConsoleController
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
                    WriteLine("Incorect input");
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
                    WriteLine("Incorect input");
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
            WriteLine("\t\tName\t\tCount\t\tPrice");
            int i = 1;
            foreach (var item in goods)
            {
                var count = item.Count;
                if (count < 0)
                {
                    count = 0;
                }
                WriteLine((i++) + "\t\t" + item.Name + "\t\t" + count + "\t\t" + item.Priсe + "\t\t");
            }
        }

        public static void ShowCategory(List<CategoryBLL> categories)
        {
            WriteLine("\tName");
            int i = 1;
            foreach (var item in categories)
            {
                WriteLine((i++) + "\t" + item.Name);
            }
        }

        public static bool YNquestion(string question)
        {
            WriteLine(question+" (y/n)");
            if (Console.ReadKey().Key == ConsoleKey.Y)
                return true;
            else
                return false;
        }
    }
}
