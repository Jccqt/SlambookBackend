using SlambookBackend.DTO.Profile;
using SlambookBackend.DTO.Slambook;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface ISlambookRepository
    {
        Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count, int userId, CancellationToken ct = default);
        Task<ServiceResponse<SlambookDetailsDTO>> GetSlambookDetails(int slambookId, CancellationToken ct = default);
        Task<ServiceResponse<List<MiniProfileDTO>>> GetSlambookResponders(int slambookId, CancellationToken ct = default);
        Task<ServiceResponse<SlambookQuestionsDTO>> GetSlambookQuestions(int slambookId, CancellationToken ct = default);
        Task<ServiceResponse<ResponderSlambookResultDTO>> GetResponderAnswers(int slambookId, int responderId, CancellationToken ct = default);
        Task<ServiceResponse> CheckSlambookOwnership(int slambookId, int responderId, CancellationToken ct = default);
        Task<ServiceResponse> CheckIfUserResponded(int slambookId, int responderId, CancellationToken ct = default);
        Task<ServiceResponse<int>> CreateSlambook(CreateSlambookDTO slambook, CancellationToken ct = default);
        Task<ServiceResponse> SubmitAnswers(SubmitAnwersDTO answers, CancellationToken ct = default);
        Task<ServiceResponse> RemoveUserResponse(int slambookId, int responderId, CancellationToken ct = default);
    }
}
