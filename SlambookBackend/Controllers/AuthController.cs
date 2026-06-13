using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.DTO.Auth;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse>> Login([FromBody] LoginRequestDTO login)
        {
            var result = await _authRepo.Login(login.Email, login.Password);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
