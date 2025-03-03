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

namespace ProjectMongoDB.IntegrationTests
{
    public class UserControllerIntegrationTests: IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<PassportUser> _passportUserCollection;
        private readonly IMongoCollection<UserImage> _userImageCollection;
        private readonly HttpClient _client;

        public UserControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();

            // Set up MongoDB client for the integration test (adjust the connection string as needed)
            var mongoClient = new MongoClient("mongodb://localhost:27017"); // Ensure MongoDB is running locally
            var database = mongoClient.GetDatabase("TestDatabase");
            _userCollection = database.GetCollection<User>("Users");
            _passportUserCollection = database.GetCollection<PassportUser>("PassportUsers");
            _userImageCollection = database.GetCollection<UserImage>("UsersImage");

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
    }
}