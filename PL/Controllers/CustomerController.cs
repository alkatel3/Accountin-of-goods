using BLL.DTO;
using BLL.Interfaces;
using BLL.Servises;
using DAL.EF;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PL.Models;

namespace PL.Controllers
{
    public class CustomerController : Controller
    {
        ICustomerService customerService;

        public CustomerController(ApplicationContext serv)
        {
            var UoW = new UnitOfWork(serv);
            customerService = new CustomerServise(UoW);
        }

        public ActionResult SignUp(UserViewModel user)
        {
            var userDTO = Mappers.UserViewUserDtoMapper.Map <UserViewModel, UserDTO>(user);
            customerService.CreatAccount(userDTO);
            return View("CustomerMenu", user);
        }

        public ActionResult CustomerMenu()
        {
            return View();
        }

        public ActionResult SignIn(UserDTO user)
        { 
            var userView = Mappers.UserDtoUserViewMapper.Map<UserDTO, UserViewModel>(user);
            return View("CustomerMenu", userView);
        }

        public ActionResult ShowGoods(UserViewModel user)
        {
            var goods = customerService.GetAllGoods();
            var viewGoods = Mappers.GoodsDtoGoodsViewMapper.Map<List<GoodsDTO>, List<GoodsViewModel>>(goods);
            return View(viewGoods);
        }

        public ActionResult ShowOrderList(UserViewModel user)
        {
            var userDTO = Mappers.UserViewUserDtoMapper.Map<UserViewModel, UserDTO>(user);
            if (userDTO.FirstName == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            var OrderList = customerService.GetOrdetList(userDTO)?.Orders;
            var OrderListView = Mappers.OrderDtoOrderViewMapper.Map<List<OrderDTO>, List<OrderViewModel>>(OrderList);
            return View(OrderListView);
        }

        public ActionResult MakeOrder(int goodsId)
        {
            return View(goodsId);
        }

        [HttpPost]
        public ActionResult MakeOrder(int goodsId, int phone, uint Count)
        {
            GoodsDTO goods = customerService.GetCurrentGoods(goodsId);
            UserDTO user = customerService.GetAccount(phone);
            if (user == null || goods == null)
            {
                return View("CustomerMenu");
            }
            customerService.CreatOrder(user, goods, Count);
            var userView = Mappers.UserDtoUserViewMapper.Map<UserDTO, UserViewModel>(user);
            return View("CustomerMenu", userView);
        }
    }
}
