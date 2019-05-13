using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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


        public List<User> GetFollowing(string userId)
        {
            var usr = Get(userId);

            return _users.Find(user => usr.Following.Contains(user.Id) & !user.Blocked.Contains(userId)).ToList();
        }

        public List<Post> GetFeed(string userId)
        {
            var following = GetFollowing(userId);

            // Find alle posts for dem der føles
            List<Post> posts = new List<Post>();
            foreach (var user in following)
            {
                posts.AddRange(user.Posts.ToList());
            }

            List<Circle> circles = _service.GetCirclesForUser(userId);

            foreach (var circle in circles)
            {
                posts.AddRange(circle.Posts.ToList());
            }

            posts.AddRange(Get(userId).Posts.ToList());

            return posts;
        }

        public List<Post> GetWall(string userId)
        {
            // Skal hente alle en users posts
            // Seneste posts, hvis ikke bruger er blokeret 
            // Hvis brugere er i samme cirkel kan posts også se tiknyttede posts



        }

        public List<User> GetBlocked(string userId)
        {
            var usr = Get(userId);
            return _users.Find(user => usr.Blocked.Contains(user.Id)).ToList();
        }


        public User Get(string id)
        {
            return _users.Find<User>(user => user.Id == id).FirstOrDefault();
        }

        public Post InsertPost(Post post)
        {
            _users.InsertOne(post);
            return post;
        }

        public void Update(string id, User userIn)
        {
            _users.ReplaceOne(user => user.Id == id, userIn);
        }

        public void Remove(User userIn)
        {
            _users.DeleteOne(user => user.Id == userIn.Id);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(User=> user.Id == id);
        }
    }
}
