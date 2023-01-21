using AutoMapper;
using BLL.DTO;

namespace PL.Models
{
    public static class Mappers
    {
        //public static IMapper GoodsDtoGoodsViewMapper = new MapperConfiguration(cfg =>
        //    cfg.CreateMap<GoodsDTO, GoodsViewModel>()).CreateMapper();

        public static IMapper OrderListDtoOrdrListViewMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<OrderListDTO, OrderListViewModel>();
        }).CreateMapper();

        public static IMapper OrderDtoOrderViewMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsDTO, GoodsViewModel>();
            cfg.CreateMap<OrderDTO, OrderViewModel>().ForMember("Goods", o => o.MapFrom(o => o.Goods));
        }).CreateMapper();

        public static IMapper UserViewUserDtoMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<OrderViewModel, OrderDTO>();
            cfg.CreateMap<OrderListViewModel, OrderListDTO>().ForMember("Orders", o => o.MapFrom(o => o.Orders));
            cfg.CreateMap<UserViewModel, UserDTO>().ForMember("OrderList", o => o.MapFrom(o => o.OrderList));
        }).CreateMapper();

        public static IMapper UserDtoUserViewMapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<GoodsDTO, GoodsViewModel>();
            cfg.CreateMap<OrderDTO, OrderViewModel>().ForMember("Goods",o=>o.MapFrom(o=>o.Goods));
            cfg.CreateMap<OrderListDTO, OrderListViewModel>().ForMember("Orders", o => o.MapFrom(o => o.Orders));
            cfg.CreateMap<UserDTO, UserViewModel>().ForMember("OrderList", o => o.MapFrom(o => o.OrderList));
        }).CreateMapper();



        public static IMapper QueueForPurchaseMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStockDTO, GoodsInStockViewModel>();
            cfg.CreateMap<GoodsDTO, GoodsViewModel>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            cfg.CreateMap<QueueForPurchaseDTO, QueueForPurchaseViewModel>().ForMember("Goods", o => o.MapFrom(qp => qp.Goods));
        }).CreateMapper();

        public static IMapper GoodsDtoGoodsViewMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStockDTO, GoodsInStockViewModel>();
            cfg.CreateMap<GoodsDTO, GoodsViewModel>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
        }).CreateMapper();

        //public static IMapper OrderDtoOrderViewMapper = new MapperConfiguration(cfg =>
        //{
        //    cfg.CreateMap<GoodsDTO, GoodsViewModel>();
        //    cfg.CreateMap<OrderDTO, OrderViewModel>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
        //}).CreateMapper();

        //public static IMapper UserViewUserDtoMapper = new MapperConfiguration(cfg =>
        //    cfg.CreateMap<UserViewModel, UserDTO>()).CreateMapper();

        //public static IMapper UserDTOUserViewModelMapper = new MapperConfiguration(cfg =>
        //    cfg.CreateMap<UserDTO, UserViewModel>()).CreateMapper();
    }
}
