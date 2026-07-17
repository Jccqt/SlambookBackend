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

        [Fact]
        public async Task GetUserById_WhenUserNotFound_ShouldReturnNullData()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUserById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUsernameById_WhenUserExists_ShouldReturnUsername()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var user = _users.Generate(1);
            user[0].Username = "Test";
            context.Add(user[0]);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUsernameById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);

            var returnedUsername = Assert.IsType<string>(result.Data);

            Assert.Equal(user[0].Username, returnedUsername);
        }

        [Fact]
        public async Task GetUsernameById_WhenUserDoesNotExists_ShouldReturnNull()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.GetUsernameById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AddUser_WhenEmailDoesNotExist_ShouldAddUserAndReturnSuccess()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();
            var repository = new UserRepository(context);

            var newUserDto = new AddUserDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "StrongPassword123!"
            };

            // Act
            var result = await repository.AddUser(newUserDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Account created successfully.", result.Message);

            var savedUser = context.Users.FirstOrDefault(u => u.Email == newUserDto.Email);
            Assert.NotNull(savedUser);
            Assert.Equal(newUserDto.FirstName, savedUser.FirstName);
            Assert.Equal(newUserDto.LastName, savedUser.LastName);
            Assert.Equal(0, savedUser.LoginCount);
            Assert.Equal(1, savedUser.Status);

            Assert.NotNull(savedUser.Salt);
            Assert.NotNull(savedUser.Password);
            Assert.NotEqual(newUserDto.Password, savedUser.Password);
        }

        [Fact]
        public async Task AddUser_WhenEmailAlreadyExists_ShouldReturnError()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var existingUser = _users.Generate(1)[0];
            existingUser.Email = "existing.email@example.com";
            context.Users.Add(existingUser);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            var duplicateUserDto = new AddUserDTO
            {
                FirstName = "Another",
                LastName = "Name",
                Email = "existing.email@example.com",
                Password = "DifferentPassword123!"
            };

            // Act
            var result = await repository.AddUser(duplicateUserDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Account already exists.", result.Message);

            var userCount = context.Users.Count();
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenUserExists_ShouldIncrementCountAndReturnSuccess()
        {
            // Arrange
            using var context = DbContextHelper.GetInMemoryContext();

            var user = _users.Generate(1)[0];
            user.LoginCount = 5;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // Act
            var result = await repository.UpdateLoginCount(user.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Successfully updated login count.", result.Message);

            var updatedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(6, updatedUser.LoginCount);
        }
    }
}
