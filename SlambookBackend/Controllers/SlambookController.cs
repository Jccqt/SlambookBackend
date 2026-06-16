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

        /// <summary>
        /// Retrieves all slambooks for a specific user. Count can be set to 0 if needed to retrieves all slambooks.
        /// </summary>
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

        /// <summary>
        /// Retrieves slambook details. Can also used this along with GetSlambookResponders to retrieve responders on this specific slambook.
        /// </summary>
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

        /// <summary>
        /// Retrieves responders list of specific slambook. This will return mini profiles similar to GetAllProfiles in ProfileController.
        /// </summary>
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

        /// <summary>
        /// Retrieves slambook questions. This will return a list of questions and can be used for displaying the questions when giving response to the slambook.
        /// </summary>
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

        /// <summary>
        /// Retrieves responder's answers. This can be used for viewing the answers of a responder for a spefiic slambook.
        /// A mini profile of the responder, and answers and questions will be returned here
        /// </summary>
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

        [HttpGet("{slambookId}/ownership")]
        public async Task<ActionResult<ServiceResponse>> CheckSlambookOwnership(
            [FromRoute] int slambookid,
            [FromQuery] int responderId)
        {
            if(slambookid <= 0 || responderId <= 0)
            {
                return BadRequest(new ServiceResponse { Message = "Invalid IDs." });
            }

            var result = await _slambookRepo.CheckSlambookOwnership(slambookid, responderId);

            return Ok(result);
        }

        /// <summary>
        /// Used for creating a slambook.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ServiceResponse>> CreateSlambook([FromBody] CreateSlambookDTO slambook)
        {
            var result = await _slambookRepo.CreateSlambook(slambook);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Used for submitting answers for a specific slambook.
        /// </summary>
        [HttpPost("answers")]
        public async Task<ActionResult<ServiceResponse>> SubmitAnswers([FromBody] SubmitAnwersDTO answers)
        {
            var result = await _slambookRepo.SubmitAnswers(answers);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Used for removing a response of a specific user in a specific slambook.
        /// </summary>
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