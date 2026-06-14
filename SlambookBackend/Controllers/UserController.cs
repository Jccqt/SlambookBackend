using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.DTO.Users;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetAllUsers()
        {
            var result = await _userRepo.GetAllUsers();

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("userId")]
        public async Task<ActionResult<ServiceResponse>> GetUserById([FromRoute] int userId)
        {
            if(userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var result = await _userRepo.GetUserById(userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddUser([FromBody] AddUserDTO user)
        {
            var result = await _userRepo.AddUser(user);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{userId}/login-count")]
        public async Task<ActionResult<ServiceResponse>> UpdateLoginCount([FromRoute] int userId)
        {
            var result = await _userRepo.UpdateLoginCount(userId);

            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
