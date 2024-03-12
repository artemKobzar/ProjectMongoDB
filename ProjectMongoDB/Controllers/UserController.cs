using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectMongoDB.Entities;
using ProjectMongoDB.Repositories;
using ProjectMongoDB.Services;
using System.Xml.Linq;

namespace ProjectMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserImageRepository _userImageRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;
        public UserController(IUserRepository userRepository, IUserImageRepository userImageRepository, IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
            _userImageRepository = userImageRepository;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userRepository.Get(id);
            if (user == null) 
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("/UserWithPassport/{id}")]
        public async Task<IActionResult> GetUserWithPassport(string id)
        {
            var user = await _userRepository.GetUserWithPassport(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] User user)
        {
            await _userRepository.Add(user);
            return Ok("User has been created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] User updatedUser)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userRepository.Update(id,updatedUser);

            return Ok("User has been updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userRepository.Delete(id);
            return NoContent();
        }

        [HttpGet]
        [Route("/UserWithPassport", Name = "UP")]
        public async Task<IActionResult> GetAllWithPassport(string? firstName, string? lastName, string? nation, string? gender)
        {
            //var authClient = _httpClientFactory.CreateClient();
            //var discoveryDocument = await authClient.GetDiscoveryDocumentAsync("https://localhost:7255");
            //var token = await authClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            //{
            //    Address = discoveryDocument.TokenEndpoint,
            //    ClientId = "project-test-api",
            //    ClientSecret = "secret"
            //});
            //var userClient = _httpClientFactory.CreateClient();
            //userClient.SetBearerToken(token.AccessToken);

            var users = await _userRepository.GetAllUserWithPassport(firstName, lastName, nation, gender);
            return Ok(users);
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromQuery] string name, string passportId, IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            {
                var fullFileName = $"{name}{file.ContentType}";
                var fileId = _userImageRepository.UploadImage(fileStream, name ?? file.Name, passportId);
                return Ok(fileId);
            }
        }

        [HttpGet("/downloadImage/{id}")]
        public async Task<IActionResult> DownloadUserImageById(string id)
        {
            var user = await _userRepository.DownloadUserImageById(id);
            if (user == null)
            {
                return NotFound();
            }
            var imageName = user.Passport.Image.Name;
            var image = _userImageRepository.DownloadImage(imageName);
            var nameFormat = $"{imageName}.jpg";

            return File(image, "application/octet-stream", nameFormat);
        }

    }
}
