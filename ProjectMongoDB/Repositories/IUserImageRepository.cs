using MongoDB.Bson;

namespace ProjectMongoDB.Repositories
{
    public interface IUserImageRepository
    {
        public Task<ObjectId> UploadImage(Stream fileStream, string fileName, string passportId);
        public byte[] DownloadImage(string name);
    }
}
