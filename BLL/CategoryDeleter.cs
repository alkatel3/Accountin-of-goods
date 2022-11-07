using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class CategoryDeleter : IDeleter<Category>
    {
        private UnitOfWork UoW;

        public CategoryDeleter(UnitOfWork uoW)
        {
            UoW = uoW;
        }

        public void Delete(Category entity)
        {
            UoW.Categories.Delete(entity.Id);
            UoW.Save();
        }
    }
}
