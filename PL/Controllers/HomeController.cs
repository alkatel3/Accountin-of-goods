using BLL.Interfaces;
using BLL.Servises;
using DAL.EF;
using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using PL.Models;

namespace PL.Controllers
{
    public class HomeController : Controller
    {
        ICustomerService customerService;

        public HomeController(ApplicationContext serv)
        {
            var UoW = new UnitOfWork(serv);
            customerService = new CustomerServise(UoW);
        }

        public ActionResult Verify()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignUp() => View();
        [HttpPost]
        //Identity and roles 
        public RedirectToActionResult SignUp(UserViewModel user)
        {
            if (user.FirstName == null || user.LastName == null || user.PhoneNumber == 0)
            {
                return RedirectToAction("Verify");
            }
            var result = RedirectToAction("SignUp", "Customer", user);
            if (user.UserStatus == DAL.Statuses.UserStatus.Seller)
            {
                result = RedirectToAction("SignUp", "Seller", user);
            }

            return result;
        }

        [HttpGet]
        public ActionResult SignIn() => View();
        [HttpPost]
        public ActionResult SignIn(int phoneNumber)
        {
            var userDTO = customerService.GetAccount(phoneNumber);
            if (userDTO == null)
            {
                return View("Verify");
            }

            var result = RedirectToAction("SignIn", "Customer", userDTO);
            if (userDTO.UserStatus == DAL.Statuses.UserStatus.Seller)
            {
                result = RedirectToAction("SignIn", "Seller", userDTO);
            }
            return result;
        }
    }
}