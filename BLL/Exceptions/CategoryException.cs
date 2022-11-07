using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Exceptions
{
    internal class CategoryException:Exception
    {
        public CategoryException() : base() { }
        public CategoryException(string message) : base(message) { }
    }
}
