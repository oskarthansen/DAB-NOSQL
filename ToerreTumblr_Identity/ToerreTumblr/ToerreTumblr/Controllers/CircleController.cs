using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
         
            IndexCircleViewModel vm = new IndexCircleViewModel()
            {Circles = new List<Circle>()};

            vm.Circles = _circleService.GetCirclesForUser(HttpContext.Session.GetString("_CurrentUserId"));


            return View(vm);
        }

        public IActionResult Create()
        {

            CircleViewModel vm = new CircleViewModel()
            {
                Users = new List<string>()
            };
            

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CircleViewModel vm)
        {
            string dummy;
            for (int i = 0; i < vm.Users.Count(); i++)
            {
                if (!_userService.CheckIfUserExist(vm.Users[i]))
                {
                    return Unauthorized();
                }

                dummy = _userService.GetUserId(vm.Users[i]);
                vm.Users[i] = dummy;
            }
            vm.Users.Add(HttpContext.Session.GetString("_CurrentUserId"));
            _circleService.CreateCircle(vm.Users,vm.Name);

            return RedirectToAction("Circle", "Circle");
        }

        public IActionResult Members(string id)
        {
            MembersViewModel vm = new MembersViewModel()
            {
                users = new List<string>()
            };

            //find users for a specific 

            return View();
        }


    }
}