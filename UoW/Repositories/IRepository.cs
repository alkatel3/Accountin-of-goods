using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UoW.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? Get(int id);
        void Creat(T item);
        void Update(T item);
        void Delete(int id);
    }
}
