using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProjectMongoDB.Entities;
using ProjectMongoDB.Repositories;

namespace ProjectMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassportUserController : ControllerBase
    {
        private readonly IPassportUserRepository _passportUserRepository;
        public PassportUserController(IPassportUserRepository passportUserRepository)
        {
            _passportUserRepository = passportUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var paspportUsers = await _passportUserRepository.GetAll();
            return Ok(paspportUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var paspportUser = await _passportUserRepository.Get(id);
            if (paspportUser == null)
            {
                return NotFound();
            }
            return Ok(paspportUser);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PassportUser passportUser)
        {
            await _passportUserRepository.Add(passportUser);
            return Ok("User has been created");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] PassportUser updatedPassportUser)
        {
            var passportUser = _passportUserRepository.Get(id);
            if (passportUser == null)
            {
                return NotFound();
            }
            await _passportUserRepository.Update(id, updatedPassportUser);

            return Ok("User has been updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var passportUser = _passportUserRepository.Get(id);
            if (passportUser == null)
            {
                return NotFound();
            }
            await _passportUserRepository.Delete(id);
            return NoContent();
        }
    }
}
