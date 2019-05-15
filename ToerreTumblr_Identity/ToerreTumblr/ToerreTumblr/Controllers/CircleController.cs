using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.AspNetCore.ResponseCaching.Internal;
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
                Users = _userService.GetUserNames(),
                UsersLogin = _userService.GetUserLogins(),
                Counter = 0
            };
            

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public User AddMemberToCircle(string id, string circleId)
        {
            var usr = _userService.GetUser(id);
            if (usr != null)
            {
                return null;
            }

            return null;
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
                Users = new List<helpclass>()
            };
            
            List<string> placeholder = new List<string>();
            placeholder = _circleService.GetUsersForCircle(id);
            
            foreach (var userId in placeholder)
            {
                vm.Users.Add(new helpclass()
                {
                    users = _userService.GetUserName(userId),
                    usersId = userId
                });
            }
            

            return View(vm); 
        }

        public IActionResult Edit(string id)
        {
            var circle =_circleService.GetCircle(id);

            return View(circle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Circle circle)
        {
            _circleService.EditCircle(circle);

            return RedirectToAction("Circle");
        }

        public IActionResult Delete(string id)
        {
            var circle = _circleService.GetCircle(id);

            return View(circle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Circle circle)
        {
            _circleService.Remove(circle.Id);

            return RedirectToAction("Circle");
        }

        



    }
}