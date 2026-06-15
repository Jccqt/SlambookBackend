using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Controllers
{
    [Route("api/slambook")]
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
            [FromQuery] int userId,
            [FromQuery] int count)
        {
            if(userId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid user ID." });
            }

            var results = await _slambookRepo.GetAllSlambooks(count, userId);

            return results.Success ? Ok(results) : NotFound(results);
        }

        [HttpGet("{slambookId}")]
        public async Task<ActionResult<ServiceResponse>> GetSlambookDetails([FromRoute] int slambookId)
        {
            if(slambookId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid slambook ID." });
            }

            var result = await _slambookRepo.GetSlambookDetails(slambookId);

            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}