using SlambookBackend.DTO.Slambook;
using SlambookBackend.Models;

namespace SlambookBackend.Interfaces
{
    public interface ISlambookRepository
    {
        Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count, int userId);
        Task<ServiceResponse<SlambookDetailsDTO>> GetSlambookDetails(int slambookId);
        Task<ServiceResponse<SlambookQuestionsDTO>> GetSlambookQuestions(int slambookId);
        Task<ServiceResponse<int>> CreateSlambook(CreateSlambookDTO slambook);
        Task<ServiceResponse> SubmitAnswers(SubmitAnwersDTO answers);
    }
}
