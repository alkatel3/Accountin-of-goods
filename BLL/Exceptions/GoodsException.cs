using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    public class GoodsException:Exception
    {
        public GoodsException() : base() { }
        public GoodsException(string message) : base(message) { }
    }
}
