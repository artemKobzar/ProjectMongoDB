using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ClientMongo
{
    public class UserImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string ImageType { get; set; }
        public DateTime UploadDate { get; set; }
        public UserImage() 
        {
            UploadDate = DateTime.UtcNow;
        }
        public string PassportUserId { get; set; }
    }
}
