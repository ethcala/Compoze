using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompozeData.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public bool DarkMode { get; set; } = false;
        public string? AuthorName { get; set; }
        public string? ProjectLayout { get; set; }
        public string? CustomColor { get; set; }
        public bool? OneNoteOnly { get; set; }
    }
}