using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.DTO.Profile;
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

        [HttpPatch("update/{userId}")]
        public async Task<ActionResult<ServiceResponse>> UpdateProfile([FromRoute] int userId, [FromForm] UpdateProfileDTO profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid model state." });
            }

            byte[] profilePictureBytes = null;
            if (profile.ProfilePicture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await profile.ProfilePicture.CopyToAsync(memoryStream);
                    profilePictureBytes = memoryStream.ToArray();
                }
            }

            var result = await _profileRepo.UpdateProfile(userId, profile.Username, profile.Bio, profilePictureBytes);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
