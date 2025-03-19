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
        private readonly FilterDefinitionBuilder<PassportUser> filterBuilderPas = Builders<PassportUser>.Filter;
        public UserRepository (IMongoClient mongoClient, IOptions<DbSettings> dbSettings)
        {
            _dbSettings = dbSettings;
            //var mongoClient = new MongoClient(_dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(_dbSettings.Value.DbName);
            _userCollection = mongoDatabase.GetCollection<User>
                (_dbSettings.Value.UsersCollectionName);
            _passportUserCollection = mongoDatabase.GetCollection<PassportUser>
                (_dbSettings.Value.PassportUsersCollectionName);
            _userImageCollection = mongoDatabase.GetCollection<UserImage>
                (_dbSettings.Value.UsersImageCollectionName);
            _gridFSBucket = new GridFSBucket(mongoDatabase);

            _gridFSBucket = new GridFSBucket(mongoDatabase);
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            Console.WriteLine("Executing FindAsync in UserRepository...");

            // Check if collection is null
            if (_userCollection == null)
            {
                Console.WriteLine("ERROR: _userCollection is NULL!");
                return new List<User>();
            }
            Console.WriteLine($"Repository _userCollection: {_userCollection.GetHashCode()}");
            var result = await _userCollection.FindAsync(_ => true);
            var users = await result.ToListAsync();

            Console.WriteLine($"Users retrieved: {users.Count}");
            return users;
            //return await _userCollection.Find(filterBuilder.Empty).ToListAsync();
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
            var filter = filterBuilder.Empty;

            if (!string.IsNullOrEmpty(firstName))
            {
                filter &= filterBuilder.Regex(u => u.FirstName, new BsonRegularExpression(firstName, "i"));
            }
            if (!string.IsNullOrEmpty(lastName))
            {
                filter &= filterBuilder.Regex(u => u.LastName, new BsonRegularExpression(lastName, "i"));
            }

            var users = await _userCollection.Find(filter).ToListAsync();
            var filteredUsers = new List<User>();

            foreach (var user in users)
            {
                var passportFilter = Builders<PassportUser>.Filter.Eq(p => p.UserId, user.Id);
                if (!string.IsNullOrEmpty(nationality))
                {
                    passportFilter &= Builders<PassportUser>.Filter.Regex(p => p.Nationality, new Regex(nationality, RegexOptions.IgnoreCase));
                }
                if (!string.IsNullOrEmpty(gender))
                {
                    passportFilter &= Builders<PassportUser>.Filter.Regex(p => p.Gender, new BsonRegularExpression($"^{gender}$", "i"));
                }
                var passportUser = await _passportUserCollection.Find(passportFilter).FirstOrDefaultAsync();
                if (passportUser != null)
                {
                    user.Passport = passportUser;

                    var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
                    passportUser.Image = userImage;
                    
                    filteredUsers.Add(user);
                }
            }
            return filteredUsers;
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
            // Find the user
            var user = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;  // Return null if user does not exist
            }
            // Find the passport for the user
            var passportUser = await _passportUserCollection.Find(u => u.UserId == user.Id).FirstOrDefaultAsync();
            if (passportUser != null)
            {
                user.Passport = passportUser;

                // Find the user's image
                var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
                if (userImage != null)
                {
                    passportUser.Image = userImage;
                }
                else
                {
                    // Assign default image
                    passportUser.Image = new UserImage
                    {
                        Name = "default.jpg",
                        ImageType = ".jpg",
                        PassportUserId = passportUser.Id
                    };
                }
            }
            return user;
        }
    }
}
//    var filter = filterBuilder.Empty;

//    var users = await _userCollection.Find(u => u.Id == id).ToListAsync();
//    foreach (var entity in users)
//    {
//        var passportUser = await _passportUserCollection.Find(u => u.UserId == entity.Id).FirstOrDefaultAsync();
//        if (passportUser != null)
//        {
//            entity.Passport = passportUser;

//            var userImage = await _userImageCollection.Find(u => u.PassportUserId == passportUser.Id).FirstOrDefaultAsync();
//            if (userImage != null)
//            {
//                passportUser.Image = userImage;

//            }
//            else
//            {
//                passportUser.Image = new UserImage
//                {
//                    //Name = "default.png",
//                    Name = "default",
//                    ImageType = ".jpg",
//                    PassportUserId = passportUser.Id
//                };
//            }
//        }
//    }
//    var user = users.Find(u => u.Id == id);
//    if (user?.Passport?.Image?.Name == "default.jpg")
//    {
//        var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "default.jpg");

//        user.Passport.Image.Name = defaultImagePath; // Reference the local path for default image
//    }
//    else
//    {
//        var image = _gridFSBucket.DownloadAsBytesByName(user.Passport.Image.Name);
//    }

//    return user;