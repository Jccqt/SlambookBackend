using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using SlambookBackend.DTO.Profile;
using SlambookBackend.DTO.Slambook;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Repository
{
    public class SlambookRepository : ISlambookRepository
    {
        private readonly AppDbContext _db;

        public SlambookRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count, int userId)
        {
            var response = new ServiceResponse<List<SlambookDTO>>();

            var sql = @"
            SELECT 
                s.Id, 
                s.title AS Title, 
                s.date_created AS CreatedDate, 
                CAST(COUNT(DISTINCT a.responder_id) AS SIGNED) AS ResponseCount
            FROM Slambooks s
            LEFT JOIN Questions q ON s.Id = q.slambook_id
            LEFT JOIN Answers a ON q.Id = a.question_id
            WHERE s.creator_id = {0}
            GROUP BY s.Id, s.title, s.date_created";

            var query = _db.Database.SqlQueryRaw<SlambookDTO>(sql, userId);

            if (count > 0)
            {
                query = query.Take(count);
            }

            var slambooks = await query.ToListAsync();

            if(slambooks.Count > 0)
            {
                response.Success = true;
                response.Message = "Slambooks found.";
                response.Data = slambooks;
            }
            else
            {
                response.Message = "No slambook found.";
            }

            return response;
        }

        public async Task<ServiceResponse<SlambookDetailsDTO>> GetSlambookDetails(int slambookId)
        {
            var response = new ServiceResponse<SlambookDetailsDTO>();

            var slambookDto = await _db.Slambooks
                .Where(s => s.Id == slambookId)
                .Select(s => new SlambookDetailsDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    CreatedDate = s.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (slambookDto == null)
            {
                response.Message = "Slambo  ok details not found.";
                response.Success = false;
                return response;
            }

            var responders = await _db.Users
                .Where(u => _db.Answers.Any(a => a.Question!.SlambookId == slambookId && a.ResponderId == u.Id))
                .Select(u => new MiniProfileDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Username = u.Username,
                    ProfilePicture = $"/api/users/profile/{u.Id}/profile-picture",
                    SlambookCount = _db.Answers
                        .Where(ans => ans.ResponderId == u.Id)
                        .Select(ans => ans.Question!.SlambookId)
                        .Distinct()
                        .Count()
                })
                .ToListAsync();

            slambookDto.Responses = responders;

            response.Success = true;
            response.Message = "Slambook details found.";
            response.Data = slambookDto;

            return response;
        }

        public async Task<ServiceResponse<SlambookQuestionsDTO>> GetSlambookQuestions(int slambookId)
        {
            var response = new ServiceResponse<SlambookQuestionsDTO>();

            var slambookQuestions = await _db.Slambooks
                .Where(s => s.Id == slambookId)
                .Select(s => new SlambookQuestionsDTO
                {
                    SlambookId = s.Id,
                    Title = s.Title,
                    Questions = s.Questions.Select(q => new QuestionItemDTO
                    {
                        QuestionId = q.Id,
                        QuestionText = q.QuestionText
                    }).ToList()
                }).FirstOrDefaultAsync();

            if(slambookQuestions == null)
            {
                response.Message = "Slambook not found.";
            }
            else
            {
                response.Success = true;
                response.Message = "Slambook questions found.";
                response.Data = slambookQuestions;
            }

            return response;
        }

        public async Task<ServiceResponse<int>> CreateSlambook(CreateSlambookDTO slambook)
        {
            var response = new ServiceResponse<int>();

            if(slambook == null)
            {
                response.Message = "Invalid slambook data.";
                return response;
            }

            var newSlambook = new Slambooks
            {
                CreatorId = slambook.CreatorId,
                Title = slambook.Title,
                Description = slambook.Description,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow),

                Questions = slambook.QuestionText.Select(text => new Questions { QuestionText = text }).ToList()
            };

            _db.Slambooks.Add(newSlambook);

            await _db.SaveChangesAsync();

            response.Success = true;
            response.Message = "Slambook created successfully.";
            response.Data = newSlambook.Id;

            return response;
        }

        public async Task<ServiceResponse> SubmitAnswers(SubmitAnwersDTO answers)
        {
            var response = new ServiceResponse();

            if (answers == null || !answers.Answers.Any())
            {
                response.Message = "No answers provided.";
                return response;
            }

            var newAnswers = answers.Answers.Select(a => new Answers
            {
                QuestionId = a.QuestionId,
                ResponderId = answers.ResponderId,
                AnswerText = a.AnswerText
            }).ToList();

            _db.Answers.AddRange(newAnswers);

            await _db.SaveChangesAsync();

            response.Success = true;
            response.Message = "Answers submitted successfully.";

            return response;
        }
    }
}
