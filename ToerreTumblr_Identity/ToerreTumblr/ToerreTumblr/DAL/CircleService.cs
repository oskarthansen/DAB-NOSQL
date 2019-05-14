using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ToerreTumblr.Models;

namespace ToerreTumblr.DAL
{
    public class CircleService
    {
        private readonly IMongoCollection<Circle> _circles;
        private readonly UserService _userService;

        public CircleService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("SocialNetwork"));
            var database = client.GetDatabase("SocialNetwork");
            _circles = database.GetCollection<Circle>("Circles");
        }

        public List<Circle> GetCirclesForUser(string userId)
        {
            return _circles.Find(circle => circle.UserIds.Contains(userId)).ToList();
        }

        public Circle GetCircle(string id)
        {
            return _circles.Find<Circle>(circle => circle.Id == id).FirstOrDefault();
        }

        public Circle Create(Circle circle)
        {
            _circles.InsertOne(circle);
            return circle;
        }

        public void Update(string id, Circle circleIn)
        {
            _circles.ReplaceOne(circle => circle.Id == id, circleIn);
        }

        public void Remove(Circle circleIn)
        {
            _circles.DeleteOne(circle => circle.Id == circleIn.Id);
        }

        public void Remove(string id)
        {
            _circles.DeleteOne(circle => circle.Id == id);
        }

        public List<Post> GetPostsForCircle(string id)
        {
            var circle = _circles.Find(c => c.Id == id).FirstOrDefault();
            if (circle != null)
            {
                return circle.Posts.ToList();
            }
            return null;
        }

        public void AddUser(string circleId, string userId)
        {
            var circle = _circles.Find(c => c.Id == circleId).FirstOrDefault();
            circle.UserIds.Append(userId);
            Update(circleId, circle);
        }

        public void RemoveUser(string circleId, string userId)
        {
            var circle = _circles.Find(c => c.Id == circleId).FirstOrDefault();
            List<string> userIds = circle.UserIds.ToList();
            userIds.Remove(userId);
            circle.UserIds = userIds.ToArray();
        }


        public void CreateCircle(List<string> users, string name)
        {
            int length = users.Count();
            string[] userArray = new string[length];

            for (int i = 0; i < length; i++)
            {
                userArray[i] = users[i];
            }

            Circle circleToInsert = new Circle()
            {
                Name = name,
                UserIds = userArray,
                Posts = new Post[0]
            };
            
            _circles.InsertOne(circleToInsert);
            
        }

        public void EditCircle(Circle circle)
        {
            var oldCircle = GetCircle(circle.Id);
            circle.Posts = oldCircle.Posts;
            Update(oldCircle.Id, circle);
        }

        public List<string> GetUsersForCircle(string id)
        {
            var userLogins = _circles.AsQueryable().Where(e => e.Id == id).SelectMany(e=>e.UserIds).ToList();

            return userLogins;
            
        }
    }
}
