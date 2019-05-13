using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ToerreTumblr.DAL;
using ToerreTumblr.Models;

namespace ToerreTumblr.Controllers
{
    public class UserController : Controller
    {
        private UserService _repo;
        public UserController(IConfiguration config)
        {
            _repo = new UserService(config);
        }
        public async Task<IActionResult> ShowFeed()
        {
            List<Post> feed = _repo.GetFeed(HttpContext.Session.GetString("UserId"));
            return View(feed);
        }

        public IActionResult AddPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(int dummy)
        {
            return RedirectToAction("ShowFeed");
        }

        public async Task<IActionResult> ShowWall(string id)
        {
            List<Post> wallPosts = _repo.GetWall(id,HttpContext.Session.GetString("UserId"));
            return View(wallPosts);
        }

        public IActionResult AddComment(string id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int id, [Bind("Text")] Comment comment)
        {
            _repo.AddComment(comment);
            return RedirectToAction("ShowFeed");
        }
    }
}