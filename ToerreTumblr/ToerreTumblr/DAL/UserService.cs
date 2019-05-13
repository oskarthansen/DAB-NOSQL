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


        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            var database = client.GetDatabase("SocialNetwork");
            _users = database.GetCollection<User>("Users");
        }


        public List<User> GetFollowing(string userId)
        {
            var usr = _users.Find<User>(user => user.Id == userId).FirstOrDefault();

            return _users.Find(user => usr.Following.Contains(user.Id)).ToList();
        }

        public List<Post> GetFeed(string userId)
        {
            
        }

        public getWall

        public List<User> GetBlocked(string userId)
        {

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
