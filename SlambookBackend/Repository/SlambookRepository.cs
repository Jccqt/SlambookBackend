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

            var sql = @"
            SELECT 
                s.Id, 
                s.title AS Title, 
                s.date_created AS CreatedDate, 
                CAST(COUNT(DISTINCT a.responder_id) AS SIGNED) AS ResponseCount
            FROM Slambooks s
            LEFT JOIN Questions q ON s.Id = q.slambook_id
            LEFT JOIN Answers a ON q.Id = a.question_id
            GROUP BY s.Id, s.title, s.date_created";

            var query = _db.Database.SqlQueryRaw<SlambookDTO>(sql);

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
    }
}
