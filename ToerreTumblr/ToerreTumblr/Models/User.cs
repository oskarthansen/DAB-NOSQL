using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToerreTumblr.Models
{
    public class User : IdentityUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name{ get; set; }

        [BsonElement("Following")]
        public string[] Following { get; set; }

        [BsonElement("Blocked")]
        public string[] Blocked { get; set; }

        [BsonElement("Post")]
        public Post[] Posts { get; set; }
    }
}
