using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using SlambookBackend.DTO.Profile;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;

namespace SlambookBackend.Repository
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly AppDbContext _db;

        public ProfileRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<List<MiniProfileDTO>>> GetAllProfiles(int count)
        {
            var response = new ServiceResponse<List<MiniProfileDTO>>();

            var profiles = await _db.Users
                .AsNoTracking()
                .Select(p => new MiniProfileDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Username = p.Username,
                    ProfilePicture = $"/api/users/{p.Id}/profile-picture"
                })
                .Take(count)
                .ToListAsync();

            if(profiles.Count > 0)
            {
                response.Success = true;
                response.Message = "Profiles found.";
                response.Data = profiles;
            }
            else
            {
                response.Message = "No profile found.";
            }

            return response;
        }
    }
}
