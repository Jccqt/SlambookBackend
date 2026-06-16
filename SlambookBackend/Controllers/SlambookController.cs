using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SlambookBackend.DTO.Slambook;
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

        [HttpGet("{slambookId}/responders")]
        public async Task<ActionResult<ServiceResponse>> GetSlambookResponders([FromRoute] int slambookId)
        {
            if(slambookId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid slambook ID." });
            }

            var result = await _slambookRepo.GetSlambookResponders(slambookId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{slambookId}/questions")]
        public async Task<ActionResult<ServiceResponse>> GetSlambookQuestions([FromRoute] int slambookId)
        {
            if (slambookId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid slambook ID." });
            }

            var result = await _slambookRepo.GetSlambookQuestions(slambookId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("{slambookId}/response/{responderId}")]
        public async Task<ActionResult<ServiceResponse>> GetResponderAnswers(
            [FromRoute] int slambookId,
            [FromRoute] int responderId)
        {
            if(slambookId <= 0 || responderId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid IDs." });
            }

            var result = await _slambookRepo.GetResponderAnswers(slambookId, responderId);

            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> CreateSlambook([FromBody] CreateSlambookDTO slambook)
        {
            var result = await _slambookRepo.CreateSlambook(slambook);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("answers")]
        public async Task<ActionResult<ServiceResponse>> SubmitAnswers([FromBody] SubmitAnwersDTO answers)
        {
            var result = await _slambookRepo.SubmitAnswers(answers);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{slambookId}/remove/{responderId}")]
        public async Task<ActionResult<ServiceResponse>> RemoveUserResponse(
            [FromRoute] int slambookId,
            [FromRoute] int responderId)
        {
            if(slambookId <= 0 || responderId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid IDs." });
            }

            var result = await _slambookRepo.RemoveUserResponse(slambookId, responderId);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}