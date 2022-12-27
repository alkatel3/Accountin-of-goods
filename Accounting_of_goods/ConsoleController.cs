using BLL.DTO;

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

        public static decimal GetPrice(string ToUser)
        {
            decimal result = 0;
            bool readed = false;
            while (!readed)
            {   
                Write(ToUser);
                readed = Decimal.TryParse(ReadLine(), out result);
                if (!readed)
                {
                    Write("Incorect input\n");
                }
            }
            return result;
        }

        public static uint GetNumber(string ToUser)
        {
            uint result = 0;
            bool readed = false;
            while (!readed)
            {
                Write(ToUser);
                readed = UInt32.TryParse(ReadLine(), out result);
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
            string result = null!;

            while (result == null)
            {
                result = ReadLine();
                if (result == null)
                {
                    Write("Incorect input\n");
                }
            }
            return result;
        }

        public static void ShowGoods(List<GoodsDTO> goods, bool withGoodsStatus)
        {
            var header = "\nNumber\t\t\tName\t\t\tPrice";
            if (withGoodsStatus)
                header+="\t\t\tStatus";

            header += '\n';
            Write(header);
            int i = 1;
            foreach (var item in goods)
            {
                var line = (i++) + "\t\t\t" +
                    item.Name + "\t\t\t" +
                    item.Priсe;

                if (withGoodsStatus)
                    line += $"\t\t\t{item.GoodsStatus}";

                line += '\n';
                Write(line);
            }
        }

        public static void ShowGoods(List<GoodsInStockDTO> goods)
        {
            var header = "\nNumber\t\t\tName\t\t\tCount\t\t\tPrice\n";
            Write(header);
            int i = 1;
            foreach (var item in goods)
            {
                var line = (i++) + "\t\t\t" +
                    item.Goods.Name + "\t\t\t" +
                    item.Count + "\t\t\t" +
                    item.Goods.Priсe + "\n";
                Write(line);
            }
        }

        public static void Write(string ToUser)
        {
            Console.Write(ToUser);
        }

        public static bool YNquestion(string question)
        {
            Write(question + " (y/n)\n");
            var key = Console.ReadKey().Key;
            if (key == ConsoleKey.Y)
                return true;
            else if(key==ConsoleKey.N)
                return false;
            else
            {
                Write("Incorect input\n");
                return YNquestion(question);
            }
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static string? ReadLine()
        {
            return Console.ReadLine();
        }

        public static ConsoleKey UserWish(string WishList)
        {
            Write(WishList);
            var result = Console.ReadKey().Key;
            return result;
        }

        public static void ShowOrderList(List<OrderDTO> orders)
        {
            if (orders == null)
            {
                Write("You orders list in empty!\n");
                return;
            }
            Console.WriteLine("Number\t\t\tName\t\t\tStatus\t\t\tPrice\t\t\tCount\t\t\tSum");
            int i = 1;
            foreach (var item in orders)
            {

                Write($"{i++}\t\t\t" +
                    $"{item.Goods.Name}\t\t" +
                    $"{item.OrderStatus}\t" +
                    $"{item.Goods.Priсe}\t\t\t" +
                    $"{item.Count}\t\t\t" +
                    $"{item.Sum}\n");
            }
        }

        public static void ShowGoodsInQueue(List<QueueForPurchaseDTO> queue)
        {
            var header = "\nNumber\t\t\tName\t\t\tCount\t\t\tPriority\n";
            Write(header);
            int i = 1;
            foreach (var item in queue)
            {
                var line = (i++) + "\t\t\t" +
                    item.Goods.Name + "\t\t\t" +
                    item.Count + "\t\t\t" +
                    item.Priority + "\n";
                Write(line);
            }
        }
    }
}
