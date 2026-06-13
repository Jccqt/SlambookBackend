using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/user/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileRepository _profileRepo;

        public ProfileController(IProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetAllProfiles([FromQuery] int count)
        {
            var result = await _profileRepo.GetAllProfiles(count);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<ServiceResponse>> GetProfileByUsername([FromQuery] string username)
        {
            var result = await _profileRepo.GetProfileByUsername(username);

            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}
