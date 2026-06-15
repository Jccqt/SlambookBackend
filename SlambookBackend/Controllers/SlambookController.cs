using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/user/{userId}/slambook")]
    [ApiController]
    public class SlambookController : ControllerBase
    {
        private readonly ISlambookRepository _slambookRepo;

        public SlambookController(ISlambookRepository slambookRepo)
        {
            _slambookRepo = slambookRepo;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse>> GetAllSlambooks(
            [FromRoute] int userId,
            [FromQuery] int count)
        {
            if(userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var results = await _slambookRepo.GetAllSlambooks(count);

            return results.Success ? Ok(results) : NotFound(results);
        }
    }
}