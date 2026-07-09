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
using Microsoft.AspNetCore.Mvc;

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

        [Fact]
        public async Task GetAllusers_WhenNoUsersFound_ShouldReturnNullData()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetAllUsers(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("No users found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUserById_WhenUserFound_ShouldReturnUserDTO()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var user = _users.Generate(1);
            user[0].FirstName = "Test";
            user[0].LastName = "Test";
            context.Add(user[0]);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUserById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);

            var returnedUser = Assert.IsType<UserDTO>(result.Data);

            Assert.Equal(user[0].Id, returnedUser.Id);
            Assert.Equal(user[0].FirstName, returnedUser.FirstName);
            Assert.Equal(user[0].LastName, returnedUser.LastName);
        }
    }
}
