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
                    ProfilePicture = $"/api/users/{p.Id}/profile-picture",
                    SlambookCount = p.Slambooks.Count()
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

        public async Task<ServiceResponse<MiniProfileDTO>> GetProfileByUsername(string username)
        {
            var response = new ServiceResponse<MiniProfileDTO>();

            var profile = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Username == username);

            if(profile != null)
            {
                response.Success = true;
                response.Message = "Profile found.";
                response.Data = new MiniProfileDTO
                {
                    Id = profile.Id,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    Username = profile.Username,
                    ProfilePicture = $"/api/users/{profile.Id}/profile-picture"
                };
            }
            else
            {
                response.Message = "No profile found.";
            }

            return response;
        }

        public async Task<ServiceResponse> UpdateProfile(int userId, string username, string bio, byte[] profilePictureBytes)
        {
            var response = new ServiceResponse();

            int affectedRow = await _db.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(u => u.Username, username)
                    .SetProperty(u => u.Bio, bio)
                    .SetProperty(u => u.ProfilePicture, u => profilePictureBytes ?? u.ProfilePicture));

            if(affectedRow == 0)
            {
                response.Message = "Failed to update profile.";
            }
            else
            {
                response.Success = true;
                response.Message = "Profile updated successfully.";
            }

            return response;
        }
    }
}
