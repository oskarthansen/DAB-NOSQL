using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using ToerreTumblr.Models;

namespace ToerreTumblr.DAL
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private CircleService _service;
         
        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            var database = client.GetDatabase("SocialNetwork");
            _users = database.GetCollection<User>("Users");
            _service = new CircleService(config);
        }


        // Skal måske laves om til at hpndtere array i stedet
        public List<User> GetFollowing(string userId)
        {
            var usr = GetUser(userId);

            if (usr.Following==null)
            {
                return new List<User>();
            }
            

            List<User> followingUsers = new List<User>();
            followingUsers = _users.Find(user => usr.Following.Contains(user.Id)).ToList();
            List<User> UsersToRemove = new List<User>();
            foreach (var user in followingUsers)
            {
                if (user.Blocked != null)
                {
                    if (user.Blocked.Contains(usr.Id))
                        UsersToRemove.Remove(user);
                }
            }
            foreach (var user in UsersToRemove)
            {
                followingUsers.Remove(user);
            }

            return followingUsers;
        }

        public List<Post> GetFeed(string userId)
        {
            var following = GetFollowing(userId);

            // Find alle posts for dem der føles
            List<Post> posts = new List<Post>();
            foreach (var user in following)
            {
                if (user.Posts != null)
                {
                    posts.AddRange(user.Posts.ToList());
                }
                
            }

            List<Circle> circles = _service.GetCirclesForUser(userId);

            foreach (var circle in circles)
            {
                if (circle.Posts != null)
                {
                    posts.AddRange(circle.Posts.ToList());
                }
                
            }

            User currentUser = GetUser(userId);
            if (currentUser.Posts != null)
            {
                posts.AddRange(currentUser.Posts);
            }
            

            return posts;
        }

        public List<Post> GetWall(string userId, string guestId)
        {
            // Skal hente alle en users posts
            // Seneste posts, hvis ikke bruger er blokeret 
            // Hvis brugere er i samme cirkel kan posts også se tiknyttede posts
            var user = GetUser(userId);
            if (user.Blocked.Contains(guestId))
                return null;

            List<Post> WallPosts = new List<Post>();
            WallPosts.AddRange(user.Posts);

            List<Circle> circles = _service.GetCirclesForUser(userId);

            foreach (var circle in circles)
            {
                if(circle.UserIds.Contains(guestId))
                    WallPosts.AddRange(circle.Posts.Where(x=>x.Author==userId).ToList());
            }
            
            return WallPosts;

        }

        public List<User> GetBlocked(string userId)
        {
            var usr = GetUser(userId);
            return _users.Find(user => usr.Blocked.Contains(user.Id)).ToList();
        }

        // Mangler funktion til addPost
        // Add comment

        public Comment AddComment(string postId, Comment newComment, string sourceId, string sharedType, string userId)
        {
            Post[] allPosts;
            var usr = GetUser(userId);

            if (sharedType!="Public")
            {
                var source = _service.GetCircle(sourceId);
                allPosts = source.Posts;
            }

            else
            {
                var source = GetUser(sourceId);
                allPosts = source.Posts;
            }

            var postToComment = allPosts.FirstOrDefault(x => x.Id.ToString() == postId);
            if (postToComment != null)
            {
                if (postToComment.Comments == null)
                {
                    postToComment.Comments=new Comment[0];
                }
                var commentsList = postToComment.Comments.ToList();
                newComment.Author = usr.Name;
                commentsList.Add(newComment);
                postToComment.Comments = commentsList.ToArray();
            }

            var postIndex = allPosts.ToList().IndexOf(postToComment);

            allPosts[postIndex] = postToComment;

            if (sharedType != "Public")
            {
                var source = _service.GetCircle(sourceId);
                source.Posts = allPosts;
                _service.Update(sourceId, source);
            }

            else
            {
                var source = GetUser(sourceId);
                source.Posts = allPosts;
                Update(sourceId, source); ;
            }
            
            return newComment;
        }

        public Comment AddPublicComment(string postId, Comment comment, string userId)
        {
            User user = _users.Find(u => u.Id == userId).FirstOrDefault();
            if (user.Posts != null)
            {
                Post post = user.Posts.FirstOrDefault(p => p.Id.ToString() == postId);
                if (post == null)
                {
                    return null;
                }
                post.Comments.Append(comment);
                Update(userId, user);
            }
            return comment;
        }

        public Post AddPost(string userId, Post post)
        {
            post.Id = ObjectId.GenerateNewId(DateTime.Now);
            post.CreationTime = DateTime.Now;
            post.SharedType = "Public";
            var usr = GetUser(userId);
            post.AuthorName = usr.Login;
            post.Author = usr.Id;
            if (usr.Posts == null)
            {
                usr.Posts = new Post[]
                {
                    post
                };
            }
            else
            {
                Post[] posts = usr.Posts;
                Post[] newPosts = new Post[posts.Length + 1]; //Added one to length
                Array.Copy(posts, newPosts, posts.Length);
                newPosts[posts.Length] = post;
                usr.Posts = newPosts;
            }
            Update(userId, usr);
            return post;

            // Hvis ikke det virker så kald update () 
        }

        public Post AddPost(string userId, Post post, string circleId)
        {

            return post;
        }

        public User GetUser(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public User GetUser(string Login, string password)
        {
            return _users.Find<User>(u => u.Login == Login && u.Password == password).FirstOrDefault();
        }

        public User UserExistInDb(string username)
        {
            return null;
        }

        public bool Login(User user)
        {
            
            string id = _users.Find(u => u.Login == user.Login && u.Password==user.Password).FirstOrDefault().Id;

            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return true;
        }

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        //public Post InsertPost(Post post)
        //{
        //    _users.InsertOne(post);
        //    return post;
        //}

        public void Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void BlockUser(string UserToBlock, string UserId)
        {
            var user = GetUser(UserId);

            user.Blocked.Append(UserToBlock);

        }

        public Circle GetCircle(string id)
        {
            return _service.GetCircle(id);
        }

        public bool CheckIfUserExist(string Login)
        {
            var usertocheck = _users.Find(u => u.Login == Login);

            if (usertocheck != null)
                return true;
            return false;
        }

        public string GetUserId(string Login)
        {
            return _users.Find(u => u.Login == Login).FirstOrDefault().Id;
        }


    }
}
