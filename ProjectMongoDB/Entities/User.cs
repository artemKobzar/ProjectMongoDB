using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectMongoDB.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public PassportUser? Passport { get; set; } 
    }
}
