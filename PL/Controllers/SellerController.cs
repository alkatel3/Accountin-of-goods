using AutoMapper;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Servises;
using DAL.EF;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PL.Models;

namespace PL.Controllers
{
    public class SellerController : Controller
    {

        ISellerServise sellerService;
        UserViewModel user;


        public SellerController(ApplicationContext serv)
        {
            var UoW = new UnitOfWork(serv);
            sellerService = new SellerServise(UoW);
        }

        public ActionResult SignUp(UserViewModel user)
        {
            this.user = user;
            var userDTO = Mappers.UserViewUserDtoMapper.Map<UserViewModel, UserDTO>(user);
            sellerService.CreatAccount(userDTO);
            return View("SellerMenu", this.user);
        }

        public ActionResult SignIn(UserDTO user)
        {
            this.user = Mappers.UserDtoUserViewMapper.Map<UserDTO, UserViewModel>(user);
            return View("SellerMenu", this.user);
        }

        public ActionResult ShowGoods()
        {
            var goods = sellerService.GetAllGoods();
            var viewGoods = Mappers.GoodsDtoGoodsViewMapper.Map<List<GoodsDTO>, List<GoodsViewModel>>(goods);
            return View(viewGoods);
        }

        public ActionResult EditGoods(int goodsId)
        {
            var goods = sellerService.GetCurrentGoods(goodsId);
            var GoodsViewModel = Mappers.GoodsDtoGoodsViewMapper.Map<GoodsDTO, GoodsViewModel>(goods);
            return View(GoodsViewModel);
        }

        [HttpPost]
        public ActionResult EditGoods(GoodsViewModel goods, uint count, string price) // investigate how to map decimal 
        {
            //Security issues 
            var goodsDTO = sellerService.GetCurrentGoods(goods.Id);
            price = price.Replace('.', ','); // move to BLL make an extention to convert using culture 
            goodsDTO.Name = goods.Name;
            goodsDTO.Price = Decimal.Parse(price);
            if (goodsDTO.GoodsInStock == null)
            {
                goodsDTO.GoodsInStock = new() { GoodsId=goodsDTO.Id };
            }
            goodsDTO.GoodsInStock.Count = count;

            sellerService.UpdateGoods(goodsDTO);
            return RedirectToAction("ShowGoods");
        }

        public ActionResult SellerMenu()
        {
            return View();
        }

        public ActionResult DeleteOrder(int goodsId)
        {
            sellerService.DeleteGoods(goodsId);
            return RedirectToAction("ShowGoods");
        }

        public ActionResult CreateGoods()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGoods([FromBody] GoodsDTO goods)
        {
            var goodsDTO = new GoodsDTO()
            {
                Price = price,
                Name = name,
                GoodsInStock = new GoodsInStockDTO() { Count = count }
            };

            sellerService.CreateGoods(goodsDTO);
            return RedirectToAction("ShowGoods");
        }

        public ActionResult ShowOrders()
        {
            var orders = sellerService.GetOrders();
            var viewOrders = Mappers.OrderDtoOrderViewMapper.Map<List<OrderDTO>, List<OrderViewModel>>(orders);
            return View(viewOrders);
        }

        public ActionResult CompleteOrder(int orderId)
        {
            var order = sellerService.GetCurrentOrder(orderId);
            sellerService.ProcessOrder(order);
            return RedirectToAction("ShowOrders");
        }

        public ActionResult ShowQueueForPurchase()
        {
            var queueForPurchase = sellerService.GetQueueForPurchase();
            var viewQueueForPurchase = Mappers.QueueForPurchaseMapper
                .Map<List<QueueForPurchaseDTO>, List<QueueForPurchaseViewModel>>(queueForPurchase);
            return View(viewQueueForPurchase);
        }
        public ActionResult BringTheseGoods(int queueId)
        {
            var current = sellerService.GetCurrentInQueueForPurchaseDTO(queueId);
            sellerService.BringGoodsFromQueueForPurchase(current);
            return RedirectToAction("ShowQueueForPurchase");
        }
    }
}
