using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace ProjectMongoDB.Entities
{
    public class PassportUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public DateOnly ValidDate { get; set; }
        public string UserId { get; set; }
        public UserImage? Image { get; set; }
    }
}
