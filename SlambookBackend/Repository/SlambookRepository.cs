using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
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

        public async Task<ServiceResponse<List<SlambookDTO>>> GetAllSlambooks(int count)
        {
            var response = new ServiceResponse<List<SlambookDTO>>();

            var query = _db.Slambooks
                .AsNoTracking()
                .Select(s => new SlambookDTO
                {
                    Id = s.Id,
                    Title = s.Title,
                    ResponseCount = s.Questions
                                     .SelectMany(q => q.Answers)
                                     .Select(a => a.ResponderId)
                                     .Distinct()
                                     .Count()
                });

            if(count > 0)
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
    }
}
