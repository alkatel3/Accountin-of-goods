using BLL.Interfaces;
using DAL.Entities;
using UoW;
using BLL.Exceptions;

namespace BLL
{
    public class CategoryController : ICreater<Category>, IDeleter<Category>, IUpDater<Category>, IGiver<Category>
    {
        private UnitOfWork UoW;

        public CategoryController(UnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Creat(Category entity)
        {
            UoW.Categories.Creat(entity);
            UoW.Save();
        }

        public void Delete(Category entity)
        {
            UoW.Categories.Delete(entity.Id);
            UoW.Save();
        }

        public void UpDate(Category entity)
        {
            UoW.Categories.Update(entity);
            UoW.Save();
        }

        public List<Category> GetAll()
        {
            return UoW.Categories.GetAll().ToList();
        }

        public Category GetCurrent(int id)
        {
            var result = UoW.Categories.Get(id);
            if (result != null)
                return result;
            throw new CategoryException("Current category didn't fint");
        }
    }
}
