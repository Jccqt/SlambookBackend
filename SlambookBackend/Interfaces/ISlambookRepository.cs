using SlambookBackend.DTO.Profile;
using SlambookBackend.DTO.Slambook;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface ISlambookRepository
    {
        Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count, int userId);
        Task<ServiceResponse<SlambookDetailsDTO>> GetSlambookDetails(int slambookId);
        Task<ServiceResponse<List<MiniProfileDTO>>> GetSlambookResponders(int slambookId);
        Task<ServiceResponse<SlambookQuestionsDTO>> GetSlambookQuestions(int slambookId);
        Task<ServiceResponse<ResponderSlambookResultDTO>> GetResponderAnswers(int slambookId, int responderId);
        Task<ServiceResponse<int>> CreateSlambook(CreateSlambookDTO slambook);
        Task<ServiceResponse> SubmitAnswers(SubmitAnwersDTO answers);
        Task<ServiceResponse> RemoveUserResponse(int slambookId, int responderId);
    }
}
