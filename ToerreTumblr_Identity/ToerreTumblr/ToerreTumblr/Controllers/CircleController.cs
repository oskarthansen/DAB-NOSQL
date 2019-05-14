using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ToerreTumblr.DAL;
using ToerreTumblr.Models;
using ToerreTumblr.ViewModels;

namespace ToerreTumblr.Controllers
{
    public class CircleController : Controller
    {

        private readonly CircleService _circleService;
        private readonly UserService _userService;

        public CircleController(IConfiguration config)
        {
            _userService = new UserService(config);
            _circleService = new CircleService(config);
        }

        public IActionResult Circle()
        {
            return View();
        }

        public IActionResult Create()
        {

            CircleViewModel vm = new CircleViewModel()
            {
                Users = new List<string>(),
                Circle = new Circle()
            };


            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CircleViewModel vm)
        {



            for (int i = 0; i < vm.Users.Count(); i++)
            {
                if (_userService.CheckIfUserExist(vm.Users[i]))
                {
                    vm.Circle.UserIds[i] = _userService.GetUserId(vm.Users[i]);
                    
                }
                else
                {
                    return RedirectToAction("Circle", "Circle");
                }
                
            }

            return RedirectToAction("Circle", "Circle");
        }
    }
}