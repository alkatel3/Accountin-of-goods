using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using BLL.Exceptions;
using BLL.Entities;
using AutoMapper;

namespace BLL
{
    public class GoodsController : ICreater<GoodsBLL>, IDeleter<GoodsBLL>, IUpDater<GoodsBLL>, IGiver<GoodsBLL>, IDisposable
    {
        public readonly IUnitOfWork UoW;

        public GoodsController(IUnitOfWork UoW)
        {
            this.UoW = UoW??throw new ArgumentNullException();
        }

        public void Creat(GoodsBLL entity)
        {
            var category = UoW.Categories.Get(entity.CategoryBLL.Id);
            var goods = new Goods() { 
                Name = entity.Name,
                Category = category,
                CategoryId=category.Id,
                Count = entity.Count,
                Priсe = entity.Priсe
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
            UoW.Goods.Update(goods);
            UoW.Save();
        }

        public List<GoodsBLL> GetAll()
        {
            var mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Category, CategoryBLL>();
                cfg.CreateMap<Goods, GoodsBLL>().ForMember("CategoryBLL", c=>c.MapFrom(g=>g.Category));
                }). CreateMapper();
            
            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.GetAll());
        }

        public GoodsBLL GetCurrent(int id)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryBLL>();
                cfg.CreateMap<Goods, GoodsBLL>().ForMember("CategoryBLL", c => c.MapFrom(g => g.Category));
            }).CreateMapper();

            var goods = mapper.Map<Goods, GoodsBLL>(UoW.Goods.Get(id));
            if (goods == null)
                throw new GoodsException("Current goods didn't fint");

            return goods;

        }

        public List<GoodsBLL> GetAllFollowing(int standard)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryBLL>();
                cfg.CreateMap<Goods, GoodsBLL>().ForMember("CategoryBLL", c => c.MapFrom(g => g.Category));
            }).CreateMapper();

            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.Find(g => g.CategoryId == standard));
        }

        public List<GoodsBLL> GetInStock()
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryBLL>();
                cfg.CreateMap<Goods, GoodsBLL>().ForMember("CategoryBLL", c => c.MapFrom(g => g.Category));
            }).CreateMapper();

            return mapper.Map<IEnumerable<Goods>, List<GoodsBLL>>(UoW.Goods.Find(g => g.Count > 0));
        }

        public void Dispose()
        {
            UoW.Dispose();
        }
    }
}
