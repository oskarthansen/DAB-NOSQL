using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToerreTumblr.Models
{
    public class Circle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("UserId")]
        public string[] UserIds { get; set; }

        [BsonElement("Posts")]
        public Post[] Posts { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
    }
}
