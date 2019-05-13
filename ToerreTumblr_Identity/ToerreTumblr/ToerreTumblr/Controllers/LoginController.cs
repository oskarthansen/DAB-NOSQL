using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using ToerreTumblr.DAL;
using ToerreTumblr.Models;

namespace ToerreTumblr.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _userService;


        public const string SessionKeyName = "_CurrentUserId";

        public LoginController(UserService userService)
        {
            _userService = userService;
        }
        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {

            if (!_userService.Login(user)) return View();
            //sets currentUserId - so other controllers kan use the current user.

            User dummy = _userService.GetUser(user.Login, user.Password);

            HttpContext.Session.SetString(SessionKeyName, dummy.Id);

            return RedirectToAction("ShowFeed", "User");

        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            _userService.Create(user);

            return RedirectToAction("Login");
        }
    }
}