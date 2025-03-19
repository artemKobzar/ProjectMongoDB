using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectMongoDB.Entities;
using ProjectMongoDB.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Xml.Linq;

namespace ProjectMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserImageRepository _userImageRepository;
        private readonly IWebHostEnvironment _env;

        public UserController(IUserRepository userRepository, IUserImageRepository userImageRepository, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _userImageRepository = userImageRepository;
            _env = env;
        }
        [HttpGet("/users")]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetUserWithPassport(string id)
        {
            var user = await _userRepository.GetUserWithPassport(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("/AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] User user)
        {
            await _userRepository.Add(user);
            return Ok("User has been created");
        }

        [HttpPut("/updateUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] User updatedUser)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userRepository.Update(id, updatedUser);

            return Ok("User has been updated");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> GetAllWithPassport(string? firstName, string? lastName, string? nationality, string? gender)
        {
            var users = await _userRepository.GetAllUserWithPassport(firstName, lastName, nationality, gender);
            return Ok(users);
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadImage([FromQuery] string name, string passportId, IFormFile file)
        {
            Console.WriteLine($"Received file: {file.FileName}, Size: {file.Length}");
            Console.WriteLine($"User: {name}, PassportId: {passportId}");
            using (var fileStream = file.OpenReadStream())
            {
                var fullFileName = $"{name}{file.ContentType}";
                var fileId = await _userImageRepository.UploadImage(fileStream, name ?? file.Name, passportId);
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
            var imageName = user.Passport?.Image?.Name;
            if (imageName == "default.jpg")
            {
                // Return the default image from wwwroot
                //var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "default.jpg");
                var defaultImagePath = Path.Combine(_env.WebRootPath, "default.jpg");

                if (!System.IO.File.Exists(defaultImagePath))
                {
                    return NotFound("Default image not found.");
                }

                var defaultImageBytes = await System.IO.File.ReadAllBytesAsync(defaultImagePath);
                return File(defaultImageBytes, "image/jpeg", "default.jpg");
            }
            var image = _userImageRepository.DownloadImage(imageName);
            var nameFormat = $"{imageName}.jpg";
            return File(image, "application/octet-stream", nameFormat);
        }
    }
}
