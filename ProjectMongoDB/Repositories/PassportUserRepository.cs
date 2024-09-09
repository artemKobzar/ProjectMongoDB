using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectMongoDB.DbContext;
using ProjectMongoDB.Entities;

namespace ProjectMongoDB.Repositories
{
    public class PassportUserRepository: IPassportUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<PassportUser> _passportUserCollection;
        private readonly IOptions<DbSettings> _dbSettings;
        private readonly FilterDefinitionBuilder<PassportUser> filterBuilder = Builders<PassportUser>.Filter;
        private readonly FilterDefinitionBuilder<User> filterBuilderU = Builders<User>.Filter;
        public PassportUserRepository(IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
            var mongoClient = new MongoClient(_dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_dbSettings.Value.DbName);
            _userCollection = mongoDatabase.GetCollection<User>
                (_dbSettings.Value.UsersCollectionName);
            _passportUserCollection = mongoDatabase.GetCollection<PassportUser>
                (_dbSettings.Value.PassportUsersCollectionName);
        }
        public async Task<IEnumerable<PassportUser>> GetAll()
        {
            return await _passportUserCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<PassportUser> Get(string id)
        {
            FilterDefinition<PassportUser> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await _passportUserCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Add(PassportUser passportUser)
        {
            await _passportUserCollection.InsertOneAsync(passportUser);
            FilterDefinition<User> filterUser = filterBuilderU.Eq(u => u.Id, passportUser.UserId);
            var update = Builders<User>.Update.Set(u => u.Passport, passportUser);
            await _userCollection.UpdateOneAsync(filterUser, update);
        }

        public async Task Update(string id, PassportUser passportUser)
        {
            if (passportUser == null)
            {
                throw new ArgumentNullException(nameof(passportUser));
            }
            FilterDefinition<PassportUser> filter = filterBuilder.Eq(existingPassportUser => existingPassportUser.Id, passportUser.Id);
            await _passportUserCollection.ReplaceOneAsync(filter, passportUser);
            FilterDefinition<User> filterUser = filterBuilderU.Eq(u => u.Id, passportUser.UserId);
            var update = Builders<User>.Update.Set(u => u.Passport, passportUser);
            await _userCollection.UpdateOneAsync(filterUser, update);
        }

        public async Task Delete(string id)
        {
            FilterDefinition<PassportUser> filter = filterBuilder.Eq(entity => entity.Id, id);
            await _passportUserCollection.DeleteOneAsync(filter);
        }
    }
}
