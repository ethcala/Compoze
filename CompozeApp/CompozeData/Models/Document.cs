using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CompozeData.Models 
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DocumentId { get; set; }
        public string ProjectId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentContent { get; set; }
        public string DocumentCategory { get; set; }
        public List<string> DocumentNotes { get; set; }
    }
}