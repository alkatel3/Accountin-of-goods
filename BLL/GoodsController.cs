using BLL.Interfaces;
using DAL.Entities;
using BLL.Exceptions;
using DAL.Repositories;
using BLL.Entities;
using AutoMapper;

namespace BLL
{
    public class GoodsController : ICreater<GoodsBLL>, IDeleter<GoodsBLL>, IUpDater<GoodsBLL>, IGiver<GoodsBLL>, IDisposable
    {
        private EFUnitOfWork UoW;

        public GoodsController(EFUnitOfWork UoW)
        {
            this.UoW = UoW;
        }

        public void Creat(GoodsBLL entity)
        {
            var category = UoW.Categories.Get(entity.CategoryBLL.Id);
            var goods = new Goods() { 
                Name = entity.Name, Category = category,
                Count = entity.Count, Priсe = entity.Priсe
            };
            UoW.Goods.Creat(goods);
            UoW.Save();
        }

        public void Delete(GoodsBLL entity)
        {
            UoW.Goods.Delete(entity.Id);
            UoW.Save();
        }

        public void UpDate(GoodsBLL entity)
        {
            var category = UoW.Categories.Get(entity.CategoryBLL.Id);
            var goods = new Goods()
            {
                Name = entity.Name,
                Category = category,
                Count = entity.Count,
                Priсe = entity.Priсe,
                Id=entity.Id
            };
            var g = UoW.Goods.Get(goods.Id);
            if (g != null)
            {
                UoW.Goods.Update(goods);
                UoW.Save();
            }
        }

        public List<GoodsBLL> GetAll()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Goods, GoodsBLL>()).CreateMapper();
            
            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.GetAll());
            UoW.Save();
        }

        public GoodsBLL GetCurrent(int id)
        {
            var result = UoW.Goods.Get(id);
            if (result != null) {
                if (result.Category == null)
                {
                    var c = UoW.Categories.Get(result.CategoryId);
                    result.Category = c;
                }
                var category = new CategoryBLL() { Id = result.Category.Id, Name = result.Category.Name };
                var goods = new GoodsBLL()
                {
                    Name = result.Name,
                    Id = result.Id,
                    Priсe = result.Priсe,
                    Count = result.Count,
                    CategoryBLL = category
                };
                UoW.Save();
                return goods;
            }

            throw new GoodsException("Current goods didn't fint");
        }

        public List<GoodsBLL> GetAllFollowing(int standard)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Goods, GoodsBLL>()).CreateMapper();

            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.Find(g => g.CategoryId == standard));
            UoW.Save();
        }

        public List<GoodsBLL> GetInStock()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Goods, GoodsBLL>()).CreateMapper();

            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.Find(g => g.Count > 0));
            UoW.Save();
        }

        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}
