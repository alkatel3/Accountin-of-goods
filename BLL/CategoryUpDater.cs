using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class CategoryUpDater : IUpDater<Category>
    {
        UnitOfWork UoW;

        public CategoryUpDater(UnitOfWork uoW)
        {
            UoW = uoW;
        }

        public void UpDate(Category entity)
        {
            UoW.Categories.Update(entity);
            UoW.Save();
        }
    }
}
