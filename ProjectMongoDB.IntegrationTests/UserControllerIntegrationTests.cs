using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using MongoDB.Driver;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProjectMongoDB.Entities;
using System.Net;
using System.Net.Http.Json;
using IdentityServer4.Models;
using System.Net.Http.Headers;
using System.Collections;
using System.Reflection;
using MongoDB.Bson;
using IdentityServer4.Test;
using MongoDB.Driver.GridFS;

namespace ProjectMongoDB.IntegrationTests
{
    public class UserControllerIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<PassportUser> _passportUserCollection;
        private readonly IMongoCollection<UserImage> _userImageCollection;
        private readonly HttpClient _client;
        private readonly GridFSBucket _gridFSBucket;
        public UserControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // Set up MongoDB client for the integration test (adjust the connection string as needed)
            var mongoClient = new MongoClient("mongodb://localhost:27017"); // Ensure MongoDB is running locally
            var database = mongoClient.GetDatabase("TestDatabase");
            _userCollection = database.GetCollection<User>("Users");
            _passportUserCollection = database.GetCollection<PassportUser>("PassportUsers");
            _userImageCollection = database.GetCollection<UserImage>("UsersImage");
            _gridFSBucket = new GridFSBucket(database);
            // Ensure the collection is clean before each test
            _userCollection.DeleteMany(FilterDefinition<User>.Empty);
            _passportUserCollection.DeleteMany(FilterDefinition<PassportUser>.Empty);
        }

        [Fact]
        public async Task GetAll_ReturnsOkWithUsers()
        {
            // Arrange: Seed some test data in the Users collection
            var testUsers = new List<User>
            {
                new User {Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jordan", PhoneNumber = "3809911111122", Email = "j@gmail.com", Address = "Chicago"},
                new User {Id = "657ad18d6060e307f58a2e44", FirstName = "Kobe", LastName = "Bryant", PhoneNumber = "3806611111122", Email = "b@gmail.com", Address = "Los Angeles"}
            };
            await _userCollection.InsertManyAsync(testUsers);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
            // Act: Send an HTTP GET request to the /users endpoint
            var response = await _client.GetAsync("/users");

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            Assert.Contains(users, u => u.FirstName == "Michael");
            Assert.Contains(users, u => u.FirstName == "Kobe");
        }

        [Fact]
        public async Task GetUser_ReturnsOkWithOneUser()
        {
            // Arrange: Seed some test data in the Users collection
            var testUsers = new List<User>
            {
                new User {Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jordan", PhoneNumber = "3809911111122", Email = "j@gmail.com", Address = "Chicago"},
                new User {Id = "657ad18d6060e307f58a2e44", FirstName = "Kobe", LastName = "Bryant", PhoneNumber = "3806611111122", Email = "b@gmail.com", Address = "Los Angeles"}
            };
            await _userCollection.InsertManyAsync(testUsers);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
            // Act: Send an HTTP GET request to the /users endpoint
            var response = await _client.GetAsync("/api/user/657ad18d6060e307f58a2e33");

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var user = await response.Content.ReadFromJsonAsync<User>();
            Assert.Equal("Michael", user.FirstName);
            
        }

        [Fact]
        public async Task GetAllWithPassport_ReturnsFilteredData()
        {
            // Arrange: Seed some test data in the Users collection
            var testUsers = new List<User>
            {
                new User {Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jordan", PhoneNumber = "3809911111122", Email = "mj@gmail.com", Address = "Chicago"},
                new User {Id = "657ad18d6060e307f58a2e22", FirstName = "Michael", LastName = "Jackson", PhoneNumber = "3809911112211", Email = "j@gmail.com", Address = "Los Angeles"},
                new User {Id = "657ad18d6060e307f58a2e44", FirstName = "Kobe", LastName = "Bryant", PhoneNumber = "3806611111122", Email = "b@gmail.com", Address = "Los Angeles"}
            };
            await _userCollection.InsertManyAsync(testUsers);

            var testPassportUsers = new List<PassportUser>
            {
                new PassportUser {Id = ObjectId.GenerateNewId().ToString(), Gender = "Male", Nationality = "American", ValidDate = new DateOnly(2028,10,11), UserId = "657ad18d6060e307f58a2e33"},
                new PassportUser {Id = ObjectId.GenerateNewId().ToString(), Gender = "Male", Nationality = "American", ValidDate = new DateOnly(2028,10,11), UserId = "657ad18d6060e307f58a2e22"},
                new PassportUser {Id = ObjectId.GenerateNewId().ToString(), Gender = "Male", Nationality = "American", ValidDate = new DateOnly(2028,10,11), UserId = "657ad18d6060e307f58a2e44"}
            };
            await _passportUserCollection.InsertManyAsync(testPassportUsers);

            // Act: Send an HTTP GET request to the endpoint
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");
            var response = await _client.GetAsync("/UserWithPassport?firstName=Michael&nationality=American");

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            Assert.Contains(users, u => u.FirstName == "Michael");
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task AddUser_ReturnOk_WhenItsCreated()
        {
            // Arrange: Create a test user
            var testUser = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                FirstName = "Michael",
                LastName = "Jordan",
                Email = "j@gmail.com",
                PhoneNumber = "3809911111122",
                Address = "Chicago"
            };
            var testPassportUser = new PassportUser
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Gender = "Male",
                Nationality = "American",
                ValidDate = new DateOnly(2028, 10, 11),
                UserId = testUser.Id.ToString()
            };
            testUser.Passport = testPassportUser;

            // Get the test client with authentication
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");

            // Act: Send POST request
            var response = await _client.PostAsJsonAsync("/AddUser", testUser);

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Equal("User has been created", responseString);

            // Verify in database
            var createdUser = await _userCollection.Find(u => u.Id == testUser.Id).FirstOrDefaultAsync();
            Assert.NotNull(createdUser);
            Assert.Equal("Michael", createdUser.FirstName);
            Assert.Equal("10/11/2028", createdUser.Passport.ValidDate.ToString());
        }

        [Fact]
        public async Task UpdateUser_ReturnOk_WhenItsUpdated()
        {
            // Arrange: Seed some test data in the Users collection
            var testUsers = new List<User>
            {
                new User {Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jordan", PhoneNumber = "3809911111122", Email = "j@gmail.com", Address = "Chicago"},
                new User {Id = "657ad18d6060e307f58a2e44", FirstName = "Kobe", LastName = "Bryant", PhoneNumber = "3806611111122", Email = "b@gmail.com", Address = "Los Angeles"}
            };
            await _userCollection.InsertManyAsync(testUsers);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");

            var user = new User { Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jackson", PhoneNumber = "3809911111122", Email = "j@gmail.com", Address = "Los Angeles" }; 
            // Act: Send an HTTP PUT request to the /updateUser endpoint
            var response = await _client.PutAsJsonAsync("/updateUser/657ad18d6060e307f58a2e33", user);

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await _client.GetAsync("/api/user/657ad18d6060e307f58a2e33");
            var updatedUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.Equal("Jackson", updatedUser.LastName);
        }

        [Fact]
        public async Task DeleteUser_ReturnNoContent()
        {
            // Arrange: Seed some test data in the Users collection
            var testUsers = new List<User>
            {
                new User {Id = "657ad18d6060e307f58a2e33", FirstName = "Michael", LastName = "Jordan", PhoneNumber = "3809911111122", Email = "j@gmail.com", Address = "Chicago"},
                new User {Id = "657ad18d6060e307f58a2e44", FirstName = "Kobe", LastName = "Bryant", PhoneNumber = "3806611111122", Email = "b@gmail.com", Address = "Los Angeles"}
            };
            await _userCollection.InsertManyAsync(testUsers);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme");

            // Act: Send an HTTP DELETE request to the /user delete endpoint
            var response = await _client.DeleteAsync("/api/user/657ad18d6060e307f58a2e33");

            // Assert: Verify the response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UploadImage_ReturnsOk_WhenFileIsUploaded()
        {
            // ✅ Ensure a PassportUser exists before uploading
            var passportId = ObjectId.GenerateNewId().ToString();
            var testPassport = new PassportUser { Id = passportId, UserId = ObjectId.GenerateNewId().ToString(), Image = null };
            await _passportUserCollection.InsertOneAsync(testPassport);
            // Arrange: Create a fake file (a small text file in memory)
            var fileName = "test.jpg";
            var fileBytes = new byte[] { 255, 216, 255, 224 }; // Minimal JPEG header
            var stream = new MemoryStream(fileBytes);
            var file = new StreamContent(stream);
            file.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            // Create multipart form data content (to simulate a real file upload)
            var formData = new MultipartFormDataContent
            {
                { file, "file", fileName }
            };

            // Add query parameters
            var name = "testUser";

            // Act: Send an HTTP POST request
            var response = await _client.PostAsync($"/api/user/uploadImage?name={name}&passportId={passportId}", formData);

            // Assert: Verify response
            response.EnsureSuccessStatusCode();  // Ensures status code is 2xx
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify the returned file ID
            var fileId = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(fileId), "File ID should not be empty");
        }

        [Fact]
        public async Task DownloadUserImageById_ReturnsFile_WhenImageExists()
        {
            // Arrange: Create test user and passport data
            var userId = ObjectId.GenerateNewId().ToString();
            var passportId = ObjectId.GenerateNewId().ToString();
            var imageName = "testUserImage";
            var testImageBytes = new byte[] { 255, 216, 255, 224 }; // Minimal JPEG header

            var testUser = new User
            {
                Id = userId,
                Passport = new PassportUser
                {
                    Id = passportId,
                    UserId = userId,
                    Image = new UserImage
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Name = imageName,
                        Size = testImageBytes.Length,
                        ImageType = ".jpg",
                        PassportUserId = passportId
                    }
                }
            };

            // Insert test user and passport into MongoDB collections
            await _userCollection.InsertOneAsync(testUser);
            await _passportUserCollection.InsertOneAsync(testUser.Passport);
            await _userImageCollection.InsertOneAsync(testUser.Passport.Image);

            // Upload the test image to GridFS
            using (var memoryStream = new MemoryStream(testImageBytes))
            {
                await _gridFSBucket.UploadFromStreamAsync(imageName, memoryStream);
            }

            // Act: Send an HTTP GET request to download the image
            var response = await _client.GetAsync($"/downloadImage/{userId}");

            // Assert: Verify the response
            response.EnsureSuccessStatusCode(); // Status should be 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType.ToString());

            // Verify the returned file contents
            var actualBytes = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(testImageBytes, actualBytes);
        }

        [Fact]
        public async Task DownloadUserImageById_ReturnsDefaultImage_WhenUserHasNoImage()
        {
            // Arrange: Create test user with no image
            var userId = ObjectId.GenerateNewId().ToString();
            var passportId = ObjectId.GenerateNewId().ToString();

            var testUser = new User
            {
                Id = userId,
                Passport = new PassportUser
                {
                    Id = passportId,
                    UserId = userId,
                    Image = new UserImage //  Ensure Image is not null before inserting!
                    {
                        Name = "default.jpg",
                        ImageType = ".jpg",
                        PassportUserId = passportId
                    }
                }
            };

            // Insert test user into MongoDB
            await _userCollection.InsertOneAsync(testUser);
            await _passportUserCollection.InsertOneAsync(testUser.Passport);

            // Act: Send an HTTP GET request to download the image
            var response = await _client.GetAsync($"/downloadImage/{userId}");

            // Assert: Verify the response

            response.EnsureSuccessStatusCode(); // Should return 200 OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/jpeg", response.Content.Headers.ContentType.ToString());
        }
    }
}
