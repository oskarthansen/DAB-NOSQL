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
            ViewData["CurrentUser"] = GetCurrentUser();
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

        public async Task<IActionResult> ShowWall(string id)
        {
            var wallPosts = _repo.GetWall(id, GetCurrentUser());

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


        public IActionResult GetUsers()
        {
            var users = _repo.GetUsers();
            return View(users);
        }

        [HttpGet]
        public List<User> GetUsersJson()
        {
            return _repo.GetUsers();
        }

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

        public IActionResult EditPost(string postId, string sourceId, string type)
        {
            var post = _repo.GetPost(postId, sourceId, type);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost([Bind("Id, Author, AuthorName, CreationTime, SharedType, SourceId, Text, Image")]Post post)
        {
            _repo.EditPost(post, false);
            return RedirectToAction("ShowFeed");
        }

        public IActionResult DeletePost(string postId, string sourceId, string type)
        {
            var post = _repo.GetPost(postId, sourceId, type);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost([Bind("Id, Author, AuthorName, CreationTime, SharedType, SourceId, Text, Image")]Post post)
        {
            _repo.DeletePost(post);
            return RedirectToAction("ShowFeed");
        }

        public IActionResult FollowUser(string id)
        {
            _repo.FollowUser(GetCurrentUser(), id);
            return RedirectToAction("ShowWall", new {id = id});
        }

        public IActionResult EditComment(string commentId, string postId, string sourceId, string type)
        {
            var post = _repo.GetPost(postId, sourceId, type);
            var comment = _repo.GetComment(commentId, post);
            var viewModel=new EditCommentViewModel()
            {
                Post = post,
                Comment = comment
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComment(EditCommentViewModel model)
        {
            _repo.EditComment(model.Comment, model.Post);
            return RedirectToAction("ShowFeed");
        }

        public IActionResult DeleteComment(string commentId, string postId, string sourceId, string type)
        {
            var post = _repo.GetPost(postId, sourceId, type);
            var comment = _repo.GetComment(commentId, post);
            var viewModel = new EditCommentViewModel()
            {
                Post = post,
                Comment = comment
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(EditCommentViewModel model)
        {
            _repo.DeleteComment(model.Comment, model.Post);
            return RedirectToAction("ShowFeed");
        }

    }
}