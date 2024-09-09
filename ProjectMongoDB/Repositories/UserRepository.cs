using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectMongoDB.DbContext;
using ProjectMongoDB.Entities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ProjectMongoDB.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly GridFSBucket _gridFSBucket;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<PassportUser> _passportUserCollection;
        private readonly IMongoCollection<UserImage> _userImageCollection;
        private readonly IOptions<DbSettings> _dbSettings;
        private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
        private readonly FilterDefinitionBuilder<PassportUser> filterBuilder1 = Builders<PassportUser>.Filter;
        public UserRepository (IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
            var mongoClient = new MongoClient(_dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_dbSettings.Value.DbName);
            _userCollection = mongoDatabase.GetCollection<User>
                (_dbSettings.Value.UsersCollectionName);
            _passportUserCollection = mongoDatabase.GetCollection<PassportUser>
                (_dbSettings.Value.PassportUsersCollectionName);
            _userImageCollection = mongoDatabase.GetCollection<UserImage>
                (_dbSettings.Value.UsersImageCollectionName);
            _gridFSBucket = new GridFSBucket(mongoDatabase);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<User> Get(string id)
        {
            FilterDefinition<User> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await _userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Add(User user)
        {
            if (user.Passport != null)
            {
                user.Passport.UserId = user.Id;
                await _passportUserCollection.InsertOneAsync(user.Passport);
            }
            await _userCollection.InsertOneAsync(user);
        }

        public async Task Update(string id, User user)
        {
            if (user == null) 
            {
                throw new ArgumentNullException(nameof(user));
            }
            FilterDefinition<User> filter = filterBuilder.Eq(existingUser => existingUser.Id, user.Id);
            await _userCollection.ReplaceOneAsync(filter, user);
            //if (user.Passport != null)
            //{
            //    FilterDefinition<PassportUser> filterPassport = filterBuilder1.Eq(p => p.UserId, user.Id);
            //    var update = Builders<PassportUser>.Update;
            //    await _passportUserCollection.UpdateOneAsync(filterPassport);
            //}

        }

        public async Task Delete(string id)
        {
            //var userDelete = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();

            //FilterDefinition<PassportUser> passportDelete = filterBuilder1.Eq(p => p.Id, id);
            //await _passportUserCollection.DeleteOneAsync(passportDelete);

            FilterDefinition<User> filter = filterBuilder.Eq(entity => entity.Id, id);
            await _userCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<User>> GetAllUserWithPassport(string? firstName, string? lastName, string? nationality, string? gender)
        {
            //var filter1 = Builders<User>.Filter.Where(u => u.Passport.Nationality == nationality);
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(firstName))
            {
                filter &= filterBuilder.Regex(u => u.FirstName, new BsonRegularExpression(firstName, "i"));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                filter &= filterBuilder.Regex(u => u.LastName, new BsonRegularExpression(lastName, "i"));
            }
            if (!string.IsNullOrEmpty(nationality))
            {
                filter = filterBuilder.Regex(u => u.Passport.Nationality, new BsonRegularExpression(nationality, "i"));
            }
            if (!string.IsNullOrEmpty(gender))
            {
                filter &= filterBuilder.Regex(u => u.Passport.Gender, new Regex(gender, RegexOptions.IgnoreCase));
            }
            var users = await _userCollection.Find(filter).ToListAsync();
            foreach (var user in users)
            {
                var passportUser = await _passportUserCollection.Find(u => u.UserId == user.Id ).FirstOrDefaultAsync();
                if (passportUser != null)
                {
                    user.Passport = passportUser;

                    var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
                    passportUser.Image = userImage;
                }
            }
            return users;
        }

        public async Task<IEnumerable<User>> GetUserWithPassport(string id)
        {
            var filter = filterBuilder.Empty;

            var users = await _userCollection.Find(u => u.Id == id).ToListAsync();
            foreach (var user in users)
            {
                var passportUser = await _passportUserCollection.Find(u => u.UserId == user.Id).FirstOrDefaultAsync();
                if (passportUser != null)
                {
                    user.Passport = passportUser;

                    var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
                    passportUser.Image = userImage;
                }
            }

            return users;
        }
        public async Task<User> DownloadUserImageById(string id)
        {
            var filter = filterBuilder.Empty;

            var users = await _userCollection.Find(u => u.Id == id).ToListAsync();
            foreach (var user in users)
            {
                var passportUser = await _passportUserCollection.Find(u => u.UserId == user.Id).FirstOrDefaultAsync();
                if (passportUser != null)
                {
                    user.Passport = passportUser;

                    var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
                    passportUser.Image = userImage;
                }
            }
            var userId = users.Find(u => u.Id == id);
            var image = _gridFSBucket.DownloadAsBytesByName(userId.Passport.Image.Name);
            return userId;
        }
    }
}
//{
//    var searchFilter = filterBuilder.Or(
//        filterBuilder.Regex(u => u.FirstName, new BsonRegularExpression(searchTerm, "i")),
//        filterBuilder.Regex(u => u.LastName, new BsonRegularExpression(searchTerm, "i"))
//        );
//    filter &= searchFilter;
//}
//if (!string.IsNullOrEmpty(searchTerm))
//{
//    filter &= filterBuilder.Where(u => u.FirstName.ToLower().Contains(searchTerm)
//    || u.LastName.ToLower().Contains(searchTerm));
//}
//if (!string.IsNullOrEmpty(searchTerm))


//if (user.Passport != null)
//{
//    user.Passport.UserId = user.Id;
//    await _passportUserCollection.InsertOneAsync(user.Passport);
//}