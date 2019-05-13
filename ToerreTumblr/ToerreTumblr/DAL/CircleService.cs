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
        private readonly IMongoCollection<Circle> _books;

        public CircleService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("BookstoreDb"));
            var database = client.GetDatabase("BookstoreDb");
            _books = database.GetCollection<Circle>("Books");
        }
    }
}
