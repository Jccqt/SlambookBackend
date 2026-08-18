using Slambook.UnitTests.DataGenerators;
using Slambook.UnitTests.Helpers;
using SlambookBackend.Context;
using SlambookBackend.DTO.Users;
using SlambookBackend.Models;
using SlambookBackend.Repository;
using SlambookBackend.Tools;

namespace Slambook.UnitTests.Repository
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UserFaker _users = new UserFaker();
        private readonly AppDbContext _context;
        private readonly UserRepository _repository;

        // xUnit builds a new instance of this class for every test, so each test
        // gets its own isolated in-memory database and a matching repository
        public UserRepositoryTests()
        {
            _context = DbContextHelper.GetInMemoryContext();
            _repository = new UserRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        private static CancellationToken CancelledToken()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            return cts.Token;
        }

        private async Task<Users> SeedUser(Action<Users>? customize = null)
        {
            var user = _users.Generate(1)[0];
            customize?.Invoke(user);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private async Task<Users> SeedUserWithPassword(string plainPassword)
        {
            return await SeedUser(u =>
            {
                u.Salt = Crypt.GenerateSalt();
                u.Password = Crypt.HashPassword(plainPassword, u.Salt);
            });
        }

        #region GetAllUsers

        [Fact]
        public async Task GetAllUsers_WhenUsersFound_ShouldReturnUserDTOList()
        {
            // Arrange
            var users = _users.Generate(10);
            _context.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsers(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Users found.", result.Message);
            Assert.NotNull(result.Data);

            var returnedUsers = Assert.IsType<List<UserDTO>>(result.Data);

            Assert.Equal(10, returnedUsers.Count);
        }

        [Fact]
        public async Task GetAllUsers_WhenUsersFound_ShouldMapEveryFieldOntoTheDTO()
        {
            // Arrange
            var user = await SeedUser(u =>
            {
                u.FirstName = "Ada";
                u.LastName = "Lovelace";
                u.Username = "ada";
                u.Bio = "Writes algorithms.";
            });

            // Act
            var result = await _repository.GetAllUsers(CancellationToken.None);

            // Assert
            var returnedUser = Assert.Single(result.Data!);

            Assert.Equal(user.Id, returnedUser.Id);
            Assert.Equal("Ada", returnedUser.FirstName);
            Assert.Equal("Lovelace", returnedUser.LastName);
            Assert.Equal("ada", returnedUser.Username);
            Assert.Equal("Writes algorithms.", returnedUser.Bio);
            Assert.Equal($"/api/users/profile/{user.Id}/profile-picture", returnedUser.ProfilePicture);
        }

        [Fact]
        public async Task GetAllUsers_WhenUsersHaveMixedStatus_ShouldReturnEveryUser()
        {
            // Arrange
            // GetAllUsers applies no status filter, so deactivated accounts are listed too
            var users = _users.Generate(4);
            users[0].Status = 0;
            users[1].Status = 1;
            users[2].Status = 0;
            users[3].Status = 1;

            _context.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllUsers(CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(4, result.Data!.Count);
        }

        [Fact]
        public async Task GetAllUsers_WhenNoUsersFound_ShouldReturnNullData()
        {
            // Act
            var result = await _repository.GetAllUsers(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("No users found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetAllUsers_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.GetAllUsers(CancelledToken()));
        }

        #endregion

        #region GetUserById

        [Fact]
        public async Task GetUserById_WhenUserFound_ShouldReturnUserDTO()
        {
            // Arrange
            var user = await SeedUser(u =>
            {
                u.FirstName = "Test";
                u.LastName = "Test";
            });

            // Act
            var result = await _repository.GetUserById(user.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("User found.", result.Message);

            var returnedUser = Assert.IsType<UserDTO>(result.Data);

            Assert.Equal(user.Id, returnedUser.Id);
            Assert.Equal(user.FirstName, returnedUser.FirstName);
            Assert.Equal(user.LastName, returnedUser.LastName);
            Assert.Equal(user.Username, returnedUser.Username);
            Assert.Equal(user.Bio, returnedUser.Bio);
        }

        [Fact]
        public async Task GetUserById_WhenUserNotFound_ShouldReturnNullData()
        {
            // Act
            var result = await _repository.GetUserById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public async Task GetUserById_WhenIdIsOutOfRange_ShouldReturnNullData(int userId)
        {
            // Arrange
            await SeedUser();

            // Act
            var result = await _repository.GetUserById(userId, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUserById_WhenMultipleUsersExist_ShouldReturnTheRequestedUser()
        {
            // Arrange
            var users = _users.Generate(5);
            _context.AddRange(users);
            await _context.SaveChangesAsync();

            var expected = users[3];

            // Act
            var result = await _repository.GetUserById(expected.Id, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expected.Id, result.Data!.Id);
            Assert.Equal(expected.Username, result.Data.Username);
        }

        [Fact]
        public async Task GetUserById_WhenUserFound_ShouldCorrectlyFormatProfilePictureUrl()
        {
            // Arrange
            await SeedUser(u => u.Id = 99);

            // Act
            var result = await _repository.GetUserById(99, CancellationToken.None);

            // Assert
            var returnedUser = Assert.IsType<UserDTO>(result.Data);
            Assert.Equal("/api/users/profile/99/profile-picture", returnedUser.ProfilePicture);
        }

        [Fact]
        public async Task GetUserById_WhenUserHasNoProfilePicture_ShouldStillReturnTheUrl()
        {
            // Arrange
            // The DTO always exposes the endpoint URL; serving a placeholder for a
            // missing picture is the controller's job, not the repository's
            var user = await SeedUser(u => u.ProfilePicture = null);

            // Act
            var result = await _repository.GetUserById(user.Id, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal($"/api/users/profile/{user.Id}/profile-picture", result.Data!.ProfilePicture);
        }

        [Fact]
        public async Task GetUserById_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var user = await SeedUser();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.GetUserById(user.Id, CancelledToken()));
        }

        #endregion

        #region GetUsernameById

        [Fact]
        public async Task GetUsernameById_WhenUserExists_ShouldReturnUsername()
        {
            // Arrange
            var user = await SeedUser(u => u.Username = "Test");

            // Act
            var result = await _repository.GetUsernameById(user.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("User found.", result.Message);

            var returnedUsername = Assert.IsType<string>(result.Data);

            Assert.Equal(user.Username, returnedUsername);
        }

        [Fact]
        public async Task GetUsernameById_WhenMultipleUsersExist_ShouldReturnTheRequestedUsersUsername()
        {
            // Arrange
            var users = _users.Generate(5);
            users[2].Username = "the-one-we-want";
            _context.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUsernameById(users[2].Id, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("the-one-we-want", result.Data);
        }

        [Fact]
        public async Task GetUsernameById_WhenUserDoesNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetUsernameById(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUsernameById_WhenUserExistsButUsernameIsEmpty_ShouldReturnUserNotFound()
        {
            // Arrange
            var user = await SeedUser(u => u.Username = string.Empty);

            // Act
            var result = await _repository.GetUsernameById(user.Id, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUsernameById_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var user = await SeedUser();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.GetUsernameById(user.Id, CancelledToken()));
        }

        #endregion

        #region AddUser

        [Fact]
        public async Task AddUser_WhenEmailDoesNotExist_ShouldAddUserAndReturnSuccess()
        {
            // Arrange
            var newUserDto = new AddUserDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "StrongPassword123!"
            };

            // Act
            var result = await _repository.AddUser(newUserDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Account created successfully.", result.Message);

            var savedUser = _context.Users.FirstOrDefault(u => u.Email == newUserDto.Email);
            Assert.NotNull(savedUser);
            Assert.Equal(newUserDto.FirstName, savedUser.FirstName);
            Assert.Equal(newUserDto.LastName, savedUser.LastName);
            Assert.Equal(newUserDto.Email, savedUser.Email);
            Assert.Equal(0, savedUser.LoginCount);
            Assert.Equal(1, savedUser.Status);

            Assert.NotNull(savedUser.Salt);
            Assert.NotNull(savedUser.Password);
            Assert.NotEqual(newUserDto.Password, savedUser.Password);
        }

        [Fact]
        public async Task AddUser_WhenEmailDoesNotExist_ShouldStoreThePasswordSaltedAndHashed()
        {
            // Arrange
            var newUserDto = new AddUserDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "StrongPassword123!"
            };

            // Act
            await _repository.AddUser(newUserDto, CancellationToken.None);

            // Assert
            // The stored hash has to be reproducible from the plain password plus the
            // stored salt, otherwise nobody could ever log in with this account
            var savedUser = _context.Users.Single();

            Assert.NotEmpty(savedUser.Salt);
            Assert.Equal(Crypt.HashPassword(newUserDto.Password, savedUser.Salt), savedUser.Password);
        }

        [Fact]
        public async Task AddUser_WhenCalledTwiceWithTheSamePassword_ShouldGenerateADifferentSaltForEachUser()
        {
            // Arrange
            var first = new AddUserDTO
            {
                FirstName = "First",
                LastName = "User",
                Email = "first@example.com",
                Password = "SharedPassword123!"
            };
            var second = new AddUserDTO
            {
                FirstName = "Second",
                LastName = "User",
                Email = "second@example.com",
                Password = "SharedPassword123!"
            };

            // Act
            await _repository.AddUser(first, CancellationToken.None);
            await _repository.AddUser(second, CancellationToken.None);

            // Assert
            // Identical passwords must never produce identical hashes
            var firstUser = _context.Users.Single(u => u.Email == first.Email);
            var secondUser = _context.Users.Single(u => u.Email == second.Email);

            Assert.NotEqual(firstUser.Salt, secondUser.Salt);
            Assert.NotEqual(firstUser.Password, secondUser.Password);
        }

        [Fact]
        public async Task AddUser_WhenEmailDoesNotExist_ShouldLeaveUsernameAndBioEmpty()
        {
            // Arrange
            var newUserDto = new AddUserDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "StrongPassword123!"
            };

            // Act
            await _repository.AddUser(newUserDto, CancellationToken.None);

            // Assert
            // Registration never picks a username, so a brand new account has an empty one
            // and GetUsernameById reports "User not found." until the profile is completed.
            // This test pins that behaviour so any change to it has to be deliberate.
            var savedUser = _context.Users.Single();
            Assert.Equal(string.Empty, savedUser.Username);
            Assert.Equal(string.Empty, savedUser.Bio);

            var usernameResult = await _repository.GetUsernameById(savedUser.Id, CancellationToken.None);
            Assert.False(usernameResult.Success);
            Assert.Equal("User not found.", usernameResult.Message);
        }

        [Fact]
        public async Task AddUser_WhenEmailAlreadyExists_ShouldReturnError()
        {
            // Arrange
            await SeedUser(u => u.Email = "existing.email@example.com");

            var duplicateUserDto = new AddUserDTO
            {
                FirstName = "Another",
                LastName = "Name",
                Email = "existing.email@example.com",
                Password = "DifferentPassword123!"
            };

            // Act
            var result = await _repository.AddUser(duplicateUserDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Account already exists.", result.Message);

            var userCount = _context.Users.Count();
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task AddUser_WhenEmailExistsWithDifferentCase_ShouldReturnError()
        {
            // Arrange
            // AuthRepository.Login matches emails case-insensitively, so registration has
            // to do the same or two accounts could claim the same login
            await SeedUser(u => u.Email = "john.doe@example.com");

            var duplicateUserDto = new AddUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "JOHN.DOE@EXAMPLE.COM", // Different case
                Password = "Password123!"
            };

            // Act
            var result = await _repository.AddUser(duplicateUserDto, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Account already exists.", result.Message);
            Assert.Equal(1, _context.Users.Count());
        }

        [Fact]
        public async Task AddUser_WhenStoredEmailIsUppercase_ShouldStillDetectTheDuplicate()
        {
            // Arrange
            // The same check has to work when it is the stored row, not the request,
            // that carries the unusual casing
            await SeedUser(u => u.Email = "JOHN.DOE@EXAMPLE.COM");

            var duplicateUserDto = new AddUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _repository.AddUser(duplicateUserDto, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Account already exists.", result.Message);
            Assert.Equal(1, _context.Users.Count());
        }

        [Fact]
        public async Task AddUser_WhenEmailAlreadyExists_ShouldNotOverwriteTheExistingAccount()
        {
            // Arrange
            var existingUser = await SeedUser(u => u.Email = "existing.email@example.com");

            var originalFirstName = existingUser.FirstName;
            var originalPassword = existingUser.Password;
            var originalSalt = existingUser.Salt;

            var duplicateUserDto = new AddUserDTO
            {
                FirstName = "Impostor",
                LastName = "Impostor",
                Email = "existing.email@example.com",
                Password = "DifferentPassword123!"
            };

            // Act
            await _repository.AddUser(duplicateUserDto, CancellationToken.None);

            // Assert
            var untouched = _context.Users.Single();
            Assert.Equal(originalFirstName, untouched.FirstName);
            Assert.Equal(originalPassword, untouched.Password);
            Assert.Equal(originalSalt, untouched.Salt);
        }

        [Fact]
        public async Task AddUser_WhenOtherUsersExist_ShouldAddWithoutModifyingThem()
        {
            // Arrange
            var users = _users.Generate(3);
            users[0].Email = "a@example.com";
            users[1].Email = "b@example.com";
            users[2].Email = "c@example.com";
            _context.AddRange(users);
            await _context.SaveChangesAsync();

            var originalLoginCounts = users.ToDictionary(u => u.Id, u => u.LoginCount);

            var newUserDto = new AddUserDTO
            {
                FirstName = "New",
                LastName = "User",
                Email = "new.user@example.com",
                Password = "Password123!"
            };

            // Act
            var result = await _repository.AddUser(newUserDto, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(4, _context.Users.Count());

            foreach (var original in originalLoginCounts)
            {
                Assert.Equal(original.Value, _context.Users.Single(u => u.Id == original.Key).LoginCount);
            }
        }

        [Fact]
        public async Task AddUser_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var newUserDto = new AddUserDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Password = "StrongPassword123!"
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.AddUser(newUserDto, CancelledToken()));

            Assert.Empty(_context.Users);
        }

        #endregion

        #region UpdateLoginCount

        [Fact]
        public async Task UpdateLoginCount_WhenUserExists_ShouldIncrementCountAndReturnSuccess()
        {
            // Arrange
            var user = await SeedUser(u => u.LoginCount = 5);

            // Act
            var result = await _repository.UpdateLoginCount(user.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Successfully updated login count.", result.Message);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(6, updatedUser.LoginCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenCountIsZero_ShouldIncrementToOne()
        {
            // Arrange
            var user = await SeedUser(u => u.LoginCount = 0);

            // Act
            var result = await _repository.UpdateLoginCount(user.Id, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(1, updatedUser.LoginCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenCalledRepeatedly_ShouldIncrementEachTime()
        {
            // Arrange
            var user = await SeedUser(u => u.LoginCount = 0);

            // Act
            for (int i = 0; i < 3; i++)
            {
                var result = await _repository.UpdateLoginCount(user.Id, CancellationToken.None);
                Assert.True(result.Success);
            }

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal(3, updatedUser.LoginCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenUserExists_ShouldNotModifyOtherUsers()
        {
            // Arrange
            var users = _users.Generate(3);
            users[0].LoginCount = 10;
            users[1].LoginCount = 20;
            users[2].LoginCount = 30;
            _context.AddRange(users);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateLoginCount(users[1].Id, CancellationToken.None);

            // Assert
            Assert.Equal(10, _context.Users.Single(u => u.Id == users[0].Id).LoginCount);
            Assert.Equal(21, _context.Users.Single(u => u.Id == users[1].Id).LoginCount);
            Assert.Equal(30, _context.Users.Single(u => u.Id == users[2].Id).LoginCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenUserDoesNotExist_ShouldReturnError()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _repository.UpdateLoginCount(nonExistentUserId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Failed to update login count. User not found.", result.Message);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenUserDoesNotExist_ShouldLeaveExistingUsersUntouched()
        {
            // Arrange
            var user = await SeedUser(u => u.LoginCount = 7);

            // Act
            var result = await _repository.UpdateLoginCount(999, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(7, _context.Users.Single(u => u.Id == user.Id).LoginCount);
        }

        [Fact]
        public async Task UpdateLoginCount_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var user = await SeedUser(u => u.LoginCount = 5);

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.UpdateLoginCount(user.Id, CancelledToken()));
        }

        #endregion

        #region UpdatePassword

        [Fact]
        public async Task UpdatePassword_WhenValidOldPassword_ShouldUpdateAndReturnSuccess()
        {
            // Arrange
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var oldSalt = user.Salt;
            var oldPasswordHash = user.Password;

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "CorrectOldPassword123!",
                NewPassword = "BrandNewPassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Password changed successfully.", result.Message);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);

            Assert.NotEqual(oldSalt, updatedUser.Salt);
            Assert.NotEqual(oldPasswordHash, updatedUser.Password);

            var expectedNewHash = Crypt.HashPassword("BrandNewPassword123!", updatedUser.Salt);
            Assert.Equal(expectedNewHash, updatedUser.Password);
        }

        [Fact]
        public async Task UpdatePassword_WhenValidOldPassword_ShouldRotateSaltSoTheOldPasswordNoLongerMatches()
        {
            // Arrange
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "CorrectOldPassword123!",
                NewPassword = "BrandNewPassword123!"
            };

            // Act
            await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.NotEqual(Crypt.HashPassword("CorrectOldPassword123!", updatedUser.Salt), updatedUser.Password);
        }

        [Fact]
        public async Task UpdatePassword_WhenNewPasswordIsTheSameAsTheOld_ShouldSucceedWithAFreshSalt()
        {
            // Arrange
            var user = await SeedUserWithPassword("SamePassword123!");

            var oldSalt = user.Salt;
            var oldPasswordHash = user.Password;

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "SamePassword123!",
                NewPassword = "SamePassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            // Reusing the same password is currently allowed; the salt still rotates, so
            // the stored hash changes even though the password did not
            Assert.True(result.Success);
            Assert.Equal("Password changed successfully.", result.Message);

            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.NotEqual(oldSalt, updatedUser.Salt);
            Assert.NotEqual(oldPasswordHash, updatedUser.Password);
            Assert.Equal(Crypt.HashPassword("SamePassword123!", updatedUser.Salt), updatedUser.Password);
        }

        [Fact]
        public async Task UpdatePassword_WhenOldPasswordIsInvalid_ShouldReturnInvalidOldPassword()
        {
            // Arrange
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "WrongPassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Invalid old password.", result.Message);
        }

        [Fact]
        public async Task UpdatePassword_WhenOldPasswordIsInvalid_ShouldLeavePasswordAndSaltUnchanged()
        {
            // Arrange
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var originalSalt = user.Salt;
            var originalPasswordHash = user.Password;

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "WrongPassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            // A rejected attempt must not touch the stored credentials at all
            var unchangedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(unchangedUser);
            Assert.Equal(originalSalt, unchangedUser.Salt);
            Assert.Equal(originalPasswordHash, unchangedUser.Password);
        }

        [Fact]
        public async Task UpdatePassword_WhenOldPasswordDiffersOnlyByCase_ShouldReturnInvalidOldPassword()
        {
            // Arrange
            // Unlike the email lookup, the password check is a hash comparison and so is
            // always case-sensitive
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "correctoldpassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid old password.", result.Message);
        }

        [Fact]
        public async Task UpdatePassword_WhenUserNotFound_ShouldReturnUserNotFound()
        {
            // Arrange
            var nonExistentUserId = 999;
            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "SomePassword123!",
                NewPassword = "NewPassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(nonExistentUserId, updateDto, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("User not found.", result.Message);
        }

        [Fact]
        public async Task UpdatePassword_WhenUserExists_ShouldNotModifyOtherUsersCredentials()
        {
            // Arrange
            var otherUser = await SeedUser();
            var otherSalt = otherUser.Salt;
            var otherPasswordHash = otherUser.Password;

            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "CorrectOldPassword123!",
                NewPassword = "BrandNewPassword123!"
            };

            // Act
            var result = await _repository.UpdatePassword(user.Id, updateDto, CancellationToken.None);

            // Assert
            Assert.True(result.Success);

            var untouched = _context.Users.Single(u => u.Id == otherUser.Id);
            Assert.Equal(otherSalt, untouched.Salt);
            Assert.Equal(otherPasswordHash, untouched.Password);
        }

        [Fact]
        public async Task UpdatePassword_WhenCancellationTokenIsTriggered_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var user = await SeedUserWithPassword("CorrectOldPassword123!");

            var updateDto = new UpdatePasswordDTO
            {
                OldPassword = "CorrectOldPassword123!",
                NewPassword = "BrandNewPassword123!"
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                _repository.UpdatePassword(user.Id, updateDto, CancelledToken()));
        }

        #endregion
    }
}
