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

        /// <summary>
        /// Retrieves all mini profiles that can be used in homepage and members page.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetAllProfiles([FromQuery] int count)
        {
            var result = await _profileRepo.GetAllProfiles(count);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Retrieves a mini profile using a username. This can be used in members page.
        /// </summary>
        [HttpGet("by-username/{username}")]
        public async Task<ActionResult<ServiceResponse>> GetProfileByUsername([FromRoute] string username)
        {
            var result = await _profileRepo.GetProfileByUsername(username);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Retrieves the profile picture bytes that can be used by Glide or any other extension packages.
        /// </summary>
        [HttpGet("{userId}/profile-picture")]
        public async Task<ActionResult<ServiceResponse>> GetProfilePictureBytes([FromRoute] int userId)
        {
            if(userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var result = await _profileRepo.GetProfilePictureBytes(userId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Updates the profile details. This should be in user controller, however, I separated this because of the profile picture.
        /// </summary>

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

            var result = await _profileRepo.UpdateProfile(userId, profile.FirstName, profile.LastName, profile.Username, profile.Bio, profilePictureBytes);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
