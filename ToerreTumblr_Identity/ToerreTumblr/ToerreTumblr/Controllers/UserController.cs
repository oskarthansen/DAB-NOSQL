using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ToerreTumblr.DAL;
using ToerreTumblr.Models;
using ToerreTumblr.ViewModels;

namespace ToerreTumblr.Controllers
{
    public class UserController : Controller
    {
        private UserService _repo;
        private CircleService _circleRepo;

        public string GetCurrentUser()
        {
            return HttpContext.Session.GetString("_CurrentUserId");
        }
        public UserController(IConfiguration config)
        {
            _repo = new UserService(config);
            _circleRepo=new CircleService(config);
        }
        public async Task<IActionResult> ShowFeed()
        {
            List<Post> feed = _repo.GetFeed(GetCurrentUser());
            return View(feed);
        }

        public IActionResult AddPost()
        {
            var circles = _circleRepo.GetCirclesForUser(GetCurrentUser());
            circles.Add(new Circle
            {
                Id = null,
                Name = "Public"
            });

            IEnumerable<SelectListItem> selectList = from c in circles
                select new SelectListItem
                {
                    Value = c.Id,
                    Text = c.Name
                };

            ViewData["SharedWith"] = new SelectList(selectList, "Value", "Text");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPost([Bind("Text, Image, SharedType")]Post post)
        {
            _repo.AddPost(GetCurrentUser(),post);
            return RedirectToAction("ShowFeed");
        }

        //public IActionResult AddPost(string circleId)
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddPost(string circleId, [Bind("Text")] Post post)
        //{
        //    post.CreationTime = DateTime.Now;
        //    post.Author = GetCurrentUser();
        //    post.AuthorName = _repo.GetUser(post.Author).Name;
        //    _repo.AddPost(post.Author, post,circleId);

        //    return RedirectToAction("ShowCircle",new {id = circleId});
        //}

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

        public IActionResult GetUsers()
        {
            var users = _repo.GetUsers();
            return View(users);
        }

        [HttpGet]
        public List<User> GetUsersJson()
        {
            return _repo.GetUserNames();
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddPublicComment(string id, [Bind("Text")] Comment comment)
        //{
        //    _repo.AddPublicComment(id,comment,HttpContext.Session.GetString("_CurrentUserId"));
        //    return RedirectToAction("ShowFeed");
        //}

        public IActionResult AddComment(string postId, string sourceId, string sharedType)
        {
            var viewModel = new AddCommentViewModel()
            {
                PostId = postId,
                SourceId = sourceId,
                SharedType = sharedType,
                Comment = new Comment()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(string postId, string sourceId, string sharedType, [Bind("Text")] Comment comment)
        {
            _repo.AddComment(postId, comment, sourceId, sharedType, GetCurrentUser());
            return RedirectToAction("ShowFeed");                                                                                                       
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