using System;
using System.Collections.Generic;
using System.Linq;
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
                Users = new List<User>(),
                Circle = new Circle()
            };
            vm.Users = _userService.GetAllUsers();
            

            return View(vm);
        }
        
    }
}