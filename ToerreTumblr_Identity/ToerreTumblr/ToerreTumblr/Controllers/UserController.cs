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
            List<Post> feed = _repo.GetFeed(HttpContext.Session.GetString("_CurrentUserId"));
            return View(feed);
        }

        public IActionResult AddPublicPost()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPublicPost([Bind("Text")]Post post)
        {
            _repo.AddPost(HttpContext.Session.GetString("_CurrentUserId"),post);
            return RedirectToAction("ShowFeed");
        }

        public IActionResult AddPost(string circleId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPost(string circleId, [Bind("Text")] Post post)
        {
            post.CreationTime = DateTime.Now;
            post.Author = HttpContext.Session.GetString("_CurrentUserId");
            post.AuthorName = _repo.GetUser(post.Author).Name;
            _repo.AddPost(post.Author, post,circleId);

            return RedirectToAction("ShowCircle",new {id = circleId});
        }

        public async Task<IActionResult> ShowWall(string id)
        {
            var wallPosts = _repo.GetWall(id, HttpContext.Session.GetString("_CurrentUserId"));

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

        public IActionResult AddPublicComment(string id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPublicComment(string id, [Bind("Text")] Comment comment)
        {
            _repo.AddPublicComment(id,comment,HttpContext.Session.GetString("_CurrentUserId"));
            return RedirectToAction("ShowFeed");
        }

        public IActionResult AddComment(string postId, string circleId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(string postId, string circleId, [Bind("Text")] Comment comment)
        {
            _repo.AddComment(postId, comment, circleId);
            return RedirectToAction("ShowCircle",new{id = circleId});
        }

        public IActionResult BlockUser(string id)
        {
            var user = _repo.GetUser(id);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BlockUser(string id, [Bind("Id")] User user)
        {
            if (user.Id==id)
            {
                _repo.BlockUser(id, HttpContext.Session.GetString("_CurrentUserId"));
                return RedirectToAction("ShowFeed");
            }

            return BadRequest();
        }

        public IActionResult ShowCircle(string id)
        {
            Circle circle = _repo.GetCircle(id);
            return View(circle);
        }
        

    }
}