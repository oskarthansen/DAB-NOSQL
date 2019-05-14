using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToerreTumblr.Models
{
    public class Comment
    {
        [BsonId]
        public string Id { get; set; }

        [BsonElement("AuthorId")]
        public string AuthorId { get; set; }

        [BsonElement("Author")]
        public string Author{ get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }


    }
}
