using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompozeData.Models
{
    public class Template 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateCategory { get; set; }
        public string TemplateContent { get; set; }
        public string? UserId { get; set; }
    }
}