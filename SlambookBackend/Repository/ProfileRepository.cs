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
                    ProfilePicture = $"/api/users/profile/{p.Id}/profile-picture",
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
                .Where(p => p.Username == username)
                .Select(p => new MiniProfileDTO
                {
                    Id = p.Id,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Username = p.Username,
                    ProfilePicture = $"/api/users/profile/{p.Id}/profile-picture",
                    SlambookCount = p.Slambooks.Count()
                }).FirstOrDefaultAsync();

            if (profile == null)
            {
                response.Message = "No profile found.";
            }
            else
            {
                response.Success = true;
                response.Message = "Profile found.";
                response.Data = profile; 
            }

            return response;
        }

        public async Task<ServiceResponse<byte[]>> GetProfilePictureBytes(int userId)
        {
            var response = new ServiceResponse<byte[]>();

            var imageBytes = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => u.ProfilePicture)
                .FirstOrDefaultAsync();

            if (imageBytes == null || imageBytes.Length == 0)
            {
                response.Message = "No profile picture found for this user.";
                return response;
            }

            response.Success = true;
            response.Message = "Profile picture retrieved successfully.";
            response.Data = imageBytes;

            return response;
        }

        public async Task<ServiceResponse> UpdateProfile(int userId, string firstName, string lastName, string username, string bio, byte[] profilePictureBytes)
        {
            var response = new ServiceResponse();

            int affectedRow = await _db.Users
                .Where(u => u.Id == userId)
                .ExecuteUpdateAsync(setter => setter
                    .SetProperty(u => u.FirstName, firstName)
                    .SetProperty(u => u.LastName, lastName)
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
