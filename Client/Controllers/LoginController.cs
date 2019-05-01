using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            var loginViewModel = new LoginViewModel
            {
                //LoggedIn = JwtHelper.LoggedIn(Request)
            };

            return View(loginViewModel);
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("jwtCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}