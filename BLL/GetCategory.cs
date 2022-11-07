using DAL.Entities;
using UoW;
using BLL.Interfaces;
using BLL.Exceptions;

namespace BLL
{
    public class GetCategory : IShow<Category>
    {
        private UnitOfWork UoW;

        public GetCategory(UnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public List<Category> GetAll()
        {
            return UoW.Categories.GetAll().ToList();
        }

        public Category GetCurrent(int id)
        {
            var result=UoW.Categories.Get(id);
            if (result != null)
                return result;
            throw new CategoryException("Current category didn't fint");
        }
    }
}
