using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompozeData.Models 
{
    public class Project
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string UserId { get; set; }
        public string AuthorName { get; set; }
        public string ProjectGenre { get; set; }
        public string DocumentLayout { get; set; } // chapters, scenes
        public string Categories { get; set; } // "Characters-Text Images-Gallery Settings-Text" [parse by spaces and dashes]
    }
}