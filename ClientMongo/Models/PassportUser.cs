using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ClientMongo
{
    public class PassportUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Nationality is required.")]
        public string Nationality { get; set; }

        [Required(ErrorMessage = "Valid date is required.")]
        public DateOnly ValidDate { get; set; }
        public string UserId { get; set; }
        public UserImage? Image { get; set; }
    }
}
