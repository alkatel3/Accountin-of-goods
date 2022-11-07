using BLL.Interfaces;
using DAL.Entities;
using UoW;

namespace BLL
{
    public class CategoryCreater : ICreater<Category>
    {
        UnitOfWork UoF;

        public CategoryCreater(UnitOfWork uoF)
        {
            UoF = uoF;
        }

        public void Creat(Category entity)
        {
            UoF.Categories.Creat(entity);
            UoF.Save();
        }
    }
}
