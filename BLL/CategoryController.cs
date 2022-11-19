using BLL.Interfaces;
using DAL.Entities;
using BLL.Exceptions;
using DAL.Repositories;
using BLL.Entities;
using AutoMapper;

namespace BLL
{
    public class CategoryController : ICreater<CategoryBLL>, IDeleter<CategoryBLL>, IUpDater<CategoryBLL>, IGiver<CategoryBLL>, IDisposable
    {
        private EFUnitOfWork UoW;

        public CategoryController(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Creat(CategoryBLL entity)
        {
            var c = new Category() { Name = entity.Name };
            UoW.Categories.Creat(c);
            UoW.Save();
        }

        public void Delete(CategoryBLL entity)
        {
            UoW.Categories.Delete(entity.Id);
            UoW.Save();
        }

        public void UpDate(CategoryBLL entity)
        {
            var c = new Category() { Name = entity.Name, Id = entity.Id };
            UoW.Categories.Update(c);
            UoW.Save();
        }

        public List<CategoryBLL> GetAll()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Category, CategoryBLL>()).CreateMapper();

            return mapper.Map <IEnumerable<Category>, List<CategoryBLL>>(UoW.Categories.GetAll());
            UoW.Save();
        }

        public CategoryBLL GetCurrent(int id)
        {
            var result = UoW.Categories.Get(id);
            if (result != null)
            {
                UoW.Save();
                return new CategoryBLL() { Name = result.Name, Id = result.Id };
            }
            throw new CategoryException("Current category didn't fint");
        }

        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}
