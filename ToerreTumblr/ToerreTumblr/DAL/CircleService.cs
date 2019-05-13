using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ToerreTumblr.Models;

namespace ToerreTumblr.DAL
{
    public class CircleService
    {
        private readonly IMongoCollection<Circle> _circles;

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
    }
}
