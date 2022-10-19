using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompozeData.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public bool DarkMode { get; set; } = false;
        public string? AuthorName { get; set; }
        public string? ProjectLayout { get; set; }
    }
}