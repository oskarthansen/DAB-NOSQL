using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToerreTumblr.Models
{
    public class Post
    {

        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }

        [BsonElement("Author")]
        public string Author { get; set; }

        [BsonElement("AuthorName")]
        public string AuthorName { get; set; }  

        [BsonElement("CreationTime")]
        public DateTime CreationTime { get; set; }

        [BsonElement("Comments")]
        public Comment[] Comments { get; set; }

        [BsonElement("SharedType")]
        public string SharedType { get; set; }


    }
}
