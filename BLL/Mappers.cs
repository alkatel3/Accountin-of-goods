using AutoMapper;
using BLL.DTO;
using DAL.Entities;

namespace BLL
{
    public static class Mappers
    {
        public static IMapper GoodsDtoGoodsMapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<QueueForPurchaseDTO, QueueForPurchase>();
            cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
            cfg.CreateMap<GoodsDTO, Goods>().ForMember("QueueForPurchase", o => o.MapFrom(g => g.QueueForPurchase));
            cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
        }).CreateMapper();

        public static IMapper GoodsGoodsDtoMapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<QueueForPurchase, QueueForPurchaseDTO>();
            cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
            cfg.CreateMap<Goods, GoodsDTO>().ForMember("QueueForPurchase", o => o.MapFrom(g => g.QueueForPurchase));
            cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
        }).CreateMapper();

        public static IMapper UserUserDTOmapper = new MapperConfiguration(cfg => {
            cfg.CreateMap<Goods, GoodsDTO>();
            cfg.CreateMap<Order, OrderDTO>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
            cfg.CreateMap<OrderList, OrderListDTO>().ForMember("Orders", o => o.MapFrom(o => o.Orders));
            cfg.CreateMap<User, UserDTO>().ForMember("OrderList", c => c.MapFrom(ol => ol.OrderList));
        }).CreateMapper();

        public static IMapper UserDTOUsermapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsDTO, Goods>();
            cfg.CreateMap<OrderDTO, Order>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
            cfg.CreateMap<OrderListDTO, OrderList>().ForMember("Orders", ol => ol.MapFrom(o => o.Orders));
            cfg.CreateMap<UserDTO, User>().ForMember("OrderList", o => o.MapFrom(u => u.OrderList));
        }).CreateMapper();

        public static IMapper GoodsInStockGoodsInStockDtoMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Goods, GoodsDTO>();
            cfg.CreateMap<GoodsInStock, GoodsInStockDTO>().ForMember("Goods", o => o.MapFrom(gs => gs.Goods));
        }).CreateMapper();

        public static IMapper GoodsInStockDtoGoodsInStockMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsDTO, Goods>();
            cfg.CreateMap<GoodsInStockDTO, GoodsInStock>().ForMember("Goods", o => o.MapFrom(gs => gs.Goods));
        }).CreateMapper();

        public static IMapper OrderListOrderListDTOMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<OrderList, OrderListDTO>();
        }).CreateMapper();

        public static IMapper QueueForPurchaseQueueForPurchaseDtoMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
            cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            cfg.CreateMap<QueueForPurchase, QueueForPurchaseDTO>().ForMember("Goods", o => o.MapFrom(qp => qp.Goods));
        }).CreateMapper();

        public static IMapper QueueForPurchaseDtoQueueForPurchaseMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
            cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            cfg.CreateMap<QueueForPurchaseDTO, QueueForPurchase>().ForMember("Goods", o => o.MapFrom(g => g.Goods));
        }).CreateMapper();

        public static IMapper OrderOrderDtoMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStock, GoodsInStockDTO>();
            cfg.CreateMap<Goods, GoodsDTO>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            cfg.CreateMap<Order, OrderDTO>().ForMember("Goods", o => o.MapFrom(o => o.Goods));
        }).CreateMapper();

        public static IMapper OrderDtoOrderMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GoodsInStockDTO, GoodsInStock>();
            cfg.CreateMap<GoodsDTO, Goods>().ForMember("GoodsInStock", o => o.MapFrom(g => g.GoodsInStock));
            cfg.CreateMap<OrderDTO, Order>().ForMember("Goods", o => o.MapFrom(o => o.Goods));
        }).CreateMapper();
    }
}
