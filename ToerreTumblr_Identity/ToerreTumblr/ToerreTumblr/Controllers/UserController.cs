using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ToerreTumblr.DAL;
using ToerreTumblr.Models;
using ToerreTumblr.ViewModels;

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
            var wallPosts = _repo.GetWall(id, HttpContext.Session.GetString("UserId"));

            if (wallPosts!=null)
            {
                var viewModel = new ShowWallViewModel()
                {
                    Posts = wallPosts,
                    User = _repo.GetUser(id)
                };

                return View(viewModel);
            }

            return NotFound();
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