using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToerreTumblr.Models
{
    public class User : IdentityUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Following")]
        public string[] Following { get; set; }

        [BsonElement("Block")]
        public string[] Block { get; set; }

        [BsonElement("Posts")]
        public Post[] Posts { get; set; }
    }
}
