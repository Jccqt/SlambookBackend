using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SlambookBackend.Repository;
using SlambookBackend.Models;
using Slambook.UnitTests.Helpers;
using Slambook.UnitTests.DataGenerators;
using SlambookBackend.DTO.Users;

namespace Slambook.UnitTests.Repository
{
    public class UserRepositoryTests
    {
        private readonly UserFaker _users = new UserFaker();

        [Fact]
        public async Task GetAllUsers_WhenUsersFound_ShouldReturnUserDTOList()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var users = _users.Generate(10);
            context.AddRange(users);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetAllUsers(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Data);

            var returnedUsers = Assert.IsType<List<UserDTO>>(result.Data);

            Assert.Equal(10, returnedUsers.Count);
        }
    }
}
