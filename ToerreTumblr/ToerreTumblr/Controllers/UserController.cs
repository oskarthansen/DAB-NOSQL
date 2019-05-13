using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ToerreTumblr.Controllers
{
    public class UserController : Controller
    {
        public async Task<IActionResult> ShowFeed()
        {
            return View();
        }

        public IActionResult AddPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost()
        {
            return RedirectToAction("ShowFeed");
        }

        public async Task<IActionResult> ShowWall(int id)
        {
            return View();
        }
    }
}