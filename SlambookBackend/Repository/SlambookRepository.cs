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

            var slambook = await _db.Slambooks
                .Where(s => s.Id == slambookId)
                .Select(s => new SlambookDetailsDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    CreatedDate = s.CreatedDate,

                    Responses = s.Questions
                        .SelectMany(q => q.Answers)
                        .Select(a => a.Responder)
                        .Distinct()
                        .Select(u => new MiniProfileDTO
                        {
                            Id = u.Id,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Username = u.Username,
                            ProfilePicture = $"/api/users/{u.Id}/profile-picture",
                            SlambookCount = _db.Answers
                            .Where(a => a.ResponderId == u.Id)
                            .Select(a => a.Question!.SlambookId)
                            .Distinct()
                            .Count()
                        }).ToList()
                }).FirstOrDefaultAsync();

            if(slambook != null)
            {
                response.Success = true;
                response.Message = "Slambook details found.";
                response.Data = slambook;
            }
            else
            {
                response.Message = "Slambook details not found.";
            }

            return response;
        }
    }
}
