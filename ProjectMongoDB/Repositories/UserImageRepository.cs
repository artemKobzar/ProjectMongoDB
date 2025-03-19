using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectMongoDB.DbContext;
using ProjectMongoDB.Entities;

namespace ProjectMongoDB.Repositories
{
    public class UserImageRepository: IUserImageRepository
    {
        private readonly IMongoCollection<PassportUser> _passportUserCollection;
        private readonly GridFSBucket _gridFSBucket;
        private readonly IMongoCollection<UserImage> _userImageCollection;
        private readonly IOptions<DbSettings> _dbSettings;
        private readonly FilterDefinitionBuilder<UserImage> filterBuilder = Builders<UserImage>.Filter;
        private readonly FilterDefinitionBuilder<PassportUser> filterBuilderPassport = Builders<PassportUser>.Filter;
        private readonly FilterDefinitionBuilder<UserImage> filterBuilderImage = Builders<UserImage>.Filter;
        public UserImageRepository(IMongoClient mongoClient, IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
            var mongoDatabase = mongoClient.GetDatabase(_dbSettings.Value.DbName);
            _userImageCollection = mongoDatabase.GetCollection<UserImage>
                (_dbSettings.Value.UsersImageCollectionName);
            _gridFSBucket = new GridFSBucket(mongoDatabase);
            _passportUserCollection = mongoDatabase.GetCollection<PassportUser>
                (_dbSettings.Value.PassportUsersCollectionName);
        }

        public async Task<ObjectId> UploadImage(Stream fileStream, string fileName, string passportId)
        {
            var fileId = await _gridFSBucket.UploadFromStreamAsync(fileName, fileStream);
            var image = new UserImage
            {
                Id = fileId.ToString(),
                Name = fileName,
                Size = fileStream.Length,
                ImageType = ".jpg",
                PassportUserId = passportId
            };

            await _userImageCollection.InsertOneAsync(image);

            FilterDefinition<PassportUser> filterP = filterBuilderPassport.Eq(p => p.Id, image.PassportUserId);
            var update = Builders<PassportUser>.Update.Set(p => p.Image, image);
            await _passportUserCollection.UpdateOneAsync(filterP, update);

            return fileId;
        }
        public byte[] DownloadImage(string name)
        {
            var image = _gridFSBucket.DownloadAsBytesByName(name);

            return image;
        }
        public async Task DeleteImage(string id)
        {
            FilterDefinition<UserImage> filterImage = filterBuilderImage.Eq(entity => entity.Id, id);
            await _userImageCollection.DeleteOneAsync(filterImage);
        }
    }
}
//FilterDefinition<PassportUser> filter = filterBuilderPassport.Eq(entity => entity.Id, id);
//var passport = await _passportUserCollection.Find(filter).FirstOrDefaultAsync();
//if (passport.Image != null)
//{
//    var image = passport.Image;
//    FilterDefinition<UserImage> filterImage = filterBuilderImage.Eq(entity => entity.Id, image.Id);
//    await _userImageCollection.DeleteOneAsync(filterImage);
//}