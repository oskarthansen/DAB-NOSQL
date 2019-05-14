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
            var currentUser = GetUser(userId);
            // Find alle posts for dem der følges
            List<Post> posts = new List<Post>();
            foreach (var user in following)
            {
                if (user.Posts != null)
                {
                    if (user.Blocked == null)
                    {
                        posts.AddRange(user.Posts.ToList());
                    }
                    else
                    {
                        if(!user.Blocked.Contains(userId))
                            posts.AddRange(user.Posts.ToList());
                    }
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
            if (currentUser.Posts != null)
            {
                posts.AddRange(currentUser.Posts);
            }
            
            var sortedPosts=posts.OrderBy(x=>x.CreationTime.TimeOfDay)
                .ThenBy(x=>x.CreationTime.Date)
                .ThenBy(x=>x.CreationTime.Year).Reverse().ToList();

            return sortedPosts;
        }

        public List<Post> GetWall(string userId, string guestId)
        {
            // Skal hente alle en users posts
            // Seneste posts, hvis ikke bruger er blokeret 
            // Hvis brugere er i samme cirkel kan posts også se tiknyttede posts
            var user = GetUser(userId);

            if (user.Blocked!=null)
            {
                if (user.Blocked.Contains(guestId))
                    return null;
            }
            

            List<Post> WallPosts = new List<Post>();

            if (user.Posts!=null)
            {
                WallPosts.AddRange(user.Posts);
            }
            

            List<Circle> circles = _service.GetCirclesForUser(userId);


            foreach (var circle in circles)
            {
                if(circle.UserIds.Contains(guestId)&&circle.Posts!=null)
                    WallPosts.AddRange(circle.Posts.Where(x=>x.Author==userId).ToList());
            }

            var sortedPosts = WallPosts.OrderBy(x => x.CreationTime.TimeOfDay)
                .ThenBy(x => x.CreationTime.Date)
                .ThenBy(x => x.CreationTime.Year).Reverse().ToList();

            return sortedPosts;

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
                var source = GetCircle(sourceId);
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
                newComment.AuthorId = usr.Id;
                newComment.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
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

        public Post AddPost(string userId, Post post)
        {
            post.Id = ObjectId.GenerateNewId(DateTime.Now).ToString();
            post.CreationTime = DateTime.Now;
            var usr = GetUser(userId);
            post.AuthorName = usr.Login;
            post.Author = usr.Id;

            if (post.SharedType==null)
            {
                post.SharedType = "Public";
                post.SourceId = usr.Id;

                if (usr.Posts == null)
                {
                    usr.Posts = new Post[0];
                }

                var usrPosts = usr.Posts.ToList();
                usrPosts.Add(post);
                usr.Posts = usrPosts.ToArray();

                Update(userId, usr);
            }
            else
            {
                var circle = GetCircle(post.SharedType);
                post.SharedType = circle.Name;
                post.SourceId = circle.Id;
                if (circle.Posts==null)
                {
                    circle.Posts=new Post[0];
                }

                var circlePosts = circle.Posts.ToList();
                circlePosts.Add(post);
                circle.Posts = circlePosts.ToArray();

                _service.Update(circle.Id, circle);
            }
            
            
            return post;

            // Hvis ikke det virker så kald update () 
        }

        public User GetUser(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public User GetUser(string Login, string password)
        {
            return _users.Find<User>(u => u.Login == Login && u.Password == password).FirstOrDefault();
        }

        public string GetUserId(string Login)
        {
            return _users.Find<User>(u => u.Login == Login).FirstOrDefault().Id;
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

        public void Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void BlockUser(string userToBlock, string UserId)
        {
            var user = GetUser(UserId);
            var UserToBlock = GetUser(userToBlock);
            if (user.Blocked == null)
            {
                user.Blocked = new string[]
                {
                    userToBlock
                };
            }
            else
            {
                if (!user.Blocked.Contains(userToBlock))
                {
                    string[] newBlocked = new string[user.Blocked.Length + 1];
                    Array.Copy(user.Blocked, newBlocked, user.Blocked.Length);
                    newBlocked[user.Blocked.Length] = userToBlock;
                    user.Blocked = newBlocked;
                }
            }

            if (user.Following != null)
            {
                if (user.Following.Contains(userToBlock))
                {
                    List<string> newFollowing = user.Following.ToList();
                    newFollowing.Remove(userToBlock);
                    user.Following = newFollowing.ToArray();
                }
            }

            List<string> newFollowers = UserToBlock.Followers.ToList();
            newFollowers.Remove(UserId);
            UserToBlock.Followers = newFollowers.ToArray();
            Update(UserId,user);
            Update(userToBlock,UserToBlock);
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

        public List<User> GetUsers()
        {
            return _users.Find(user => true).ToList();
        }

        public Post GetPost(string postId, string sourceId, string type)
        {
            var postList = type=="Public" ? GetUser(sourceId).Posts.ToList() : GetCircle(sourceId).Posts.ToList();

            var post = postList.FirstOrDefault(x => x.Id == postId);

            return post;
        }

        public void EditPost(Post post)
        {
            var postList = post.SharedType == "Public" ? GetUser(post.SourceId).Posts.ToList() : GetCircle(post.SourceId).Posts.ToList();
            var oldPost = GetPost(post.Id, post.SourceId, post.SharedType);
            post.Comments = oldPost.Comments;

            var postIndex = postList.FindIndex(x => x.Id == post.Id);
            postList[postIndex] = post;

            if (post.SharedType=="Public")
            {
                var usr = GetUser(post.SourceId);
                usr.Posts = postList.ToArray();
                Update(post.Author, usr);
            }

            else
            {
                var circle = GetCircle(post.SourceId);
                circle.Posts = postList.ToArray();
                _service.Update(post.SourceId, circle);
            }
        }

        public void DeletePost(Post post)
        {
            var postList = post.SharedType == "Public" ? GetUser(post.SourceId).Posts.ToList() : GetCircle(post.SourceId).Posts.ToList();

            postList.RemoveAt(postList.FindIndex(x=>x.Id==post.Id));

            if (post.SharedType == "Public")
            {
                var usr = GetUser(post.SourceId);
                usr.Posts = postList.ToArray();
                Update(post.Author, usr);
            }

            else
            {
                var circle = GetCircle(post.SourceId);
                circle.Posts = postList.ToArray();
                _service.Update(post.SourceId, circle);
            }
        }

        public List<string> GetUserNames()
        {
            var userNames = _users.AsQueryable()
                .Where(e => e.Name.Length > 0)
                .Select(e => e.Name)
                .ToList();

            return userNames;
		}			

        public void FollowUser(string currentUserId, string userToFollowId)
        {
            var currentUser = _users.Find(u => u.Id == currentUserId).FirstOrDefault();
            var UserToFollow = _users.Find(u => u.Id == userToFollowId).FirstOrDefault();
            if (currentUser == null | UserToFollow == null)
            {
                return;
            }
            //Update current user
            if (currentUser.Following == null)
            {
                currentUser.Following = new string[]
                {
                    userToFollowId
                };
            }
            else
            {
                if (!currentUser.Following.Contains(userToFollowId))
                {
                    string[] newFollowing = new string[currentUser.Following.Length + 1];
                    Array.Copy(currentUser.Following, newFollowing, currentUser.Following.Length);
                    newFollowing[currentUser.Following.Length] = userToFollowId;
                    currentUser.Following = newFollowing;
                }
            }

            if (currentUser.Blocked != null)
            {
                if (currentUser.Blocked.Contains(userToFollowId))
                {
                    List<string> newBlocked = currentUser.Blocked.ToList();
                    newBlocked.Remove(userToFollowId);
                    currentUser.Blocked = newBlocked.ToArray();
                }
            }

            //Add user to followers list of userToFollow
            if (UserToFollow.Followers == null)
            {
                UserToFollow.Followers = new string[]
                {
                    userToFollowId
                };
            }
            else
            {
                if (!UserToFollow.Followers.Contains(currentUserId))
                {
                    string[] newFollowers = new string[UserToFollow.Followers.Length + 1];
                    Array.Copy(UserToFollow.Followers, newFollowers, UserToFollow.Followers.Length);
                    newFollowers[UserToFollow.Followers.Length] = currentUserId;
                    UserToFollow.Followers = newFollowers;
                }
            }
            Update(currentUserId,currentUser);
            Update(userToFollowId,UserToFollow);
        }

    }
}
