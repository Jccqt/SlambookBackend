using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using SlambookBackend.DTO.Auth;
using SlambookBackend.Interfaces;
using SlambookBackend.Models;
using SlambookBackend.Tools;

namespace SlambookBackend.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _db;

        public AuthRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<LoginResponseDTO>> Login(string email, string password)
        {
            var response = new ServiceResponse<LoginResponseDTO>();

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if(user == null || user.Status == 0)
            {
                response.Message = "Login failed. User does not exists";
                return response;
            }

            string hashedInputPassword = Crypt.HashPassword(password, user.Salt);

            if(hashedInputPassword != user.Password)
            {
                response.Message = "Login failed. Invalid email or password.";
                return response;
            }

            response.Success = true;
            response.Message = "Login successful.";
            response.Data = new LoginResponseDTO
            {
                Id = user.Id,
                LoginCount = user.LoginCount
            };

            return response;
        }
    }
}
