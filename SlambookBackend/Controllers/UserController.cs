using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SlambookBackend.DTO.Users;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/user")]
    [ApiController]
    [EnableRateLimiting("FixedWindowPolicy")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetAllUsers(CancellationToken ct)
        {
            var result = await _userRepo.GetAllUsers(ct);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<ServiceResponse>> GetUserById([FromRoute] int userId, CancellationToken ct)
        {
            if (userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var result = await _userRepo.GetUserById(userId, ct);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{userId}/username")]
        public async Task<ActionResult<ServiceResponse>> GetUsernameById([FromRoute] int userId, CancellationToken ct)
        {
            if (userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var result = await _userRepo.GetUsernameById(userId, ct);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> AddUser([FromBody] AddUserDTO user, CancellationToken ct)
        {
            var result = await _userRepo.AddUser(user, ct);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{userId}/login-count")]
        public async Task<ActionResult<ServiceResponse>> UpdateLoginCount([FromRoute] int userId, CancellationToken ct)
        {
            var result = await _userRepo.UpdateLoginCount(userId, ct);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPatch("{userId}/password")]
        public async Task<ActionResult<ServiceResponse>> UpdatePassword(
            [FromRoute] int userId,
            [FromBody] UpdatePasswordDTO password,
            CancellationToken ct)
        {
            if(userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var result = await _userRepo.UpdatePassword(userId, password, ct);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
