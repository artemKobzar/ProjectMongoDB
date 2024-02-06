using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectMongoDB.Repositories;

namespace ProjectMongoDB.Controllers
{
    public class UserImageController: ControllerBase
    {
        private readonly IUserImageRepository _userImageRepository;
        public UserImageController(IUserImageRepository userImageRepository)
        {
            _userImageRepository = userImageRepository;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromQuery] string name, string passportId, IFormFile file)
        {
            using (var fileStream = file.OpenReadStream()) 
            {
                var fullFileName = $"{name}{file.ContentType}";
                var fileId = await _userImageRepository.UploadImage(fileStream, name ?? file.Name, passportId);
                return Ok(fileId);
            }
        }
        [HttpGet("download/{name}")]
        public async Task<IActionResult> DownloadImage(string name)
        {
            var image = _userImageRepository.DownloadImage(name);
            if (image == null)
            {
                return NotFound();
            }
            var nameFormat = $"{name}.jpg";
            return File(image, "application/octet-stream", nameFormat);
        }
    }
}
