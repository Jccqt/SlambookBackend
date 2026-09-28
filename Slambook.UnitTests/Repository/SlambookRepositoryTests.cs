using Microsoft.EntityFrameworkCore;
using Slambook.UnitTests.DataGenerators;
using Slambook.UnitTests.Helpers;
using SlambookBackend.Context;
using SlambookBackend.DTO.Profile;
using SlambookBackend.DTO.Slambook;
using SlambookBackend.Models;
using SlambookBackend.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Slambook.UnitTests.Repository
{
    public class SlambookRepositoryTests : IDisposable
    {
        private readonly SlambookFaker _slambooks = new SlambookFaker();
        private readonly AppDbContext _context;
        private readonly SlambookRepository _repository;

        public SlambookRepositoryTests()
        {
            _context = DbContextHelper.GetInMemoryContext();
            _repository = new SlambookRepository(_context);
        }

        public void Dispose() => _context.Dispose();

        private async Task<Slambooks> SeedSlambook(Action<Slambooks>? customize = null)
        {
            var slambook = _slambooks.Generate(1)[0];
            customize?.Invoke(slambook);

            _context.Slambooks.Add(slambook);
            await _context.SaveChangesAsync();

            return slambook;
        }

        #region GetAllSlambooks

        [Fact]
        public async Task GetAllSlambooks_WhenSlambooksFound_ShouldReturnSlambookDTOList()
        {
            // Arrange
            int userId = 1;
            var slambooks = _slambooks.Generate(5);
            foreach (var slambook in slambooks)
            {
                slambook.CreatorId = userId;
            }
            
            _context.AddRange(slambooks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllSlambooks(0, userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Slambooks found.", result.Message);
            Assert.NotNull(result.Data);

            var returnedSlambooks = Assert.IsType<List<SlambookDTO>>(result.Data);
            Assert.Equal(5, returnedSlambooks.Count);
        }

        [Fact]
        public async Task GetAllSlambooks_WhenNoSlambooksFound_ShouldReturnEmptyDataAndSuccessFalse()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = await _repository.GetAllSlambooks(0, userId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("No slambook found.", result.Message);
            Assert.Null(result.Data);
        }

        #endregion

        #region CheckSlambookOwnership

        [Fact]
        public async Task CheckSlambookOwnership_WhenResponderIsCreator_ShouldReturnSuccessTrue()
        {
            // Arrange
            const int creatorId = 42;
            var slambook = await SeedSlambook(s => s.CreatorId = creatorId);

            // Act
            var result = await _repository.CheckSlambookOwnership(
                slambook.Id,
                creatorId,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("This user is the owner of this slambook.", result.Message);
        }

        [Fact]
        public async Task CheckSlambookOwnership_WhenResponderIsNotCreator_ShouldReturnSuccessFalse()
        {
            // Arrange
            const int creatorId = 42;
            const int responderId = 99;
            var slambook = await SeedSlambook(s => s.CreatorId = creatorId);

            // Act
            var result = await _repository.CheckSlambookOwnership(
                slambook.Id,
                responderId,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("This user is not the owner of this slambook.", result.Message);
        }

        #endregion

        #region GetSlambookQuestions

        [Fact]
        public async Task GetSlambookQuestions_WhenSlambookFound_ShouldReturnProjectedQuestions()
        {
            // Arrange
            var slambook = await SeedSlambook(s =>
            {
                s.Title = "Friends Forever";
                s.Questions = new List<Questions>
                {
                    new Questions
                    {
                        Id = 101,
                        QuestionText = "What is your favorite memory?"
                    },
                    new Questions
                    {
                        Id = 102,
                        QuestionText = "Where do you see yourself in five years?"
                    }
                };
            });

            // Act
            var result = await _repository.GetSlambookQuestions(
                slambook.Id,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Slambook questions found.", result.Message);

            var data = Assert.IsType<SlambookQuestionsDTO>(result.Data);
            Assert.Equal(slambook.Id, data.SlambookId);
            Assert.Equal("Friends Forever", data.Title);
            Assert.Collection(
                data.Questions.OrderBy(question => question.QuestionId),
                question =>
                {
                    Assert.Equal(101, question.QuestionId);
                    Assert.Equal("What is your favorite memory?", question.QuestionText);
                },
                question =>
                {
                    Assert.Equal(102, question.QuestionId);
                    Assert.Equal("Where do you see yourself in five years?", question.QuestionText);
                });
        }

        [Fact]
        public async Task GetSlambookQuestions_WhenSlambookHasNoQuestions_ShouldReturnEmptyQuestionList()
        {
            // Arrange
            var slambook = await SeedSlambook(s => s.Title = "New Slambook");

            // Act
            var result = await _repository.GetSlambookQuestions(
                slambook.Id,
                CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Slambook questions found.", result.Message);

            var data = Assert.IsType<SlambookQuestionsDTO>(result.Data);
            Assert.Equal(slambook.Id, data.SlambookId);
            Assert.Equal("New Slambook", data.Title);
            Assert.Empty(data.Questions);
        }

        [Fact]
        public async Task GetSlambookQuestions_WhenSlambookNotFound_ShouldReturnNullDataAndSuccessFalse()
        {
            // Act
            var result = await _repository.GetSlambookQuestions(999, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Slambook not found.", result.Message);
            Assert.Null(result.Data);
        }

        #endregion

        #region GetSlambookResponders

        [Fact]
        public async Task GetSlambookResponders_WhenNoActiveRespondersFound_ShouldReturnEmptyList()
        {
            // Arrange
            const int slambookId = 42;
            var targetQuestion = new Questions
            {
                Id = 1,
                SlambookId = slambookId,
                QuestionText = "What is your favorite color?"
            };
            var otherQuestion = new Questions
            {
                Id = 2,
                SlambookId = 99,
                QuestionText = "What is your favorite food?"
            };

            _context.Questions.AddRange(targetQuestion, otherQuestion);
            _context.Answers.AddRange(
                new Answers
                {
                    Id = 1,
                    QuestionId = targetQuestion.Id,
                    ResponderId = 10,
                    AnswerText = "Blue",
                    Status = 0
                },
                new Answers
                {
                    Id = 2,
                    QuestionId = otherQuestion.Id,
                    ResponderId = 11,
                    AnswerText = "Pizza",
                    Status = 1
                });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSlambookResponders(
                slambookId,
                CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("No responders found.", result.Message);

            var responders = Assert.IsType<List<MiniProfileDTO>>(result.Data);
            Assert.Empty(responders);
        }

        [Fact]
        public async Task GetSlambookResponders_WhenActiveRespondersFound_ShouldReturnDistinctProjectedProfiles()
        {
            // Arrange
            const int slambookId = 42;
            var firstResponder = new Users
            {
                Id = 10,
                FirstName = "Ada",
                LastName = "Lovelace",
                Username = "ada",
                Slambooks = new List<Slambooks>()
            };
            var secondResponder = new Users
            {
                Id = 11,
                FirstName = "Grace",
                LastName = "Hopper",
                Username = "grace",
                Slambooks = new List<Slambooks>()
            };
            var inactiveResponder = new Users
            {
                Id = 12,
                FirstName = "Inactive",
                LastName = "Responder",
                Username = "inactive",
                Slambooks = new List<Slambooks>()
            };
            var otherSlambookResponder = new Users
            {
                Id = 13,
                FirstName = "Other",
                LastName = "Slambook",
                Username = "other",
                Slambooks = new List<Slambooks>()
            };

            _context.Users.AddRange(
                firstResponder,
                secondResponder,
                inactiveResponder,
                otherSlambookResponder);
            _context.Slambooks.AddRange(
                new Slambooks { Id = 101, CreatorId = firstResponder.Id, Title = "Ada 1" },
                new Slambooks { Id = 102, CreatorId = firstResponder.Id, Title = "Ada 2" },
                new Slambooks { Id = 103, CreatorId = secondResponder.Id, Title = "Grace 1" });

            var firstQuestion = new Questions
            {
                Id = 201,
                SlambookId = slambookId,
                QuestionText = "Question 1"
            };
            var secondQuestion = new Questions
            {
                Id = 202,
                SlambookId = slambookId,
                QuestionText = "Question 2"
            };
            var otherQuestion = new Questions
            {
                Id = 203,
                SlambookId = 99,
                QuestionText = "Other slambook question"
            };
            _context.Questions.AddRange(firstQuestion, secondQuestion, otherQuestion);
            _context.Answers.AddRange(
                new Answers { Id = 301, QuestionId = firstQuestion.Id, ResponderId = firstResponder.Id, Status = 1 },
                new Answers { Id = 302, QuestionId = secondQuestion.Id, ResponderId = firstResponder.Id, Status = 1 },
                new Answers { Id = 303, QuestionId = firstQuestion.Id, ResponderId = secondResponder.Id, Status = 1 },
                new Answers { Id = 304, QuestionId = firstQuestion.Id, ResponderId = inactiveResponder.Id, Status = 0 },
                new Answers { Id = 305, QuestionId = otherQuestion.Id, ResponderId = otherSlambookResponder.Id, Status = 1 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetSlambookResponders(
                slambookId,
                CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Found 2 responders.", result.Message);

            var responders = Assert.IsType<List<MiniProfileDTO>>(result.Data);
            Assert.Equal(2, responders.Count);

            var ada = Assert.Single(responders, responder => responder.Id == firstResponder.Id);
            Assert.Equal("Ada", ada.FirstName);
            Assert.Equal("Lovelace", ada.LastName);
            Assert.Equal("ada", ada.Username);
            Assert.Equal("/api/user/profile/10/profile-picture", ada.ProfilePicture);
            Assert.Equal(2, ada.SlambookCount);

            var grace = Assert.Single(responders, responder => responder.Id == secondResponder.Id);
            Assert.Equal("Grace", grace.FirstName);
            Assert.Equal("Hopper", grace.LastName);
            Assert.Equal("grace", grace.Username);
            Assert.Equal("/api/user/profile/11/profile-picture", grace.ProfilePicture);
            Assert.Equal(1, grace.SlambookCount);

            Assert.DoesNotContain(responders, responder => responder.Id == inactiveResponder.Id);
            Assert.DoesNotContain(responders, responder => responder.Id == otherSlambookResponder.Id);
        }

        #endregion

        #region GetSlambookDetails

        [Fact]
        public async Task GetSlambookDetails_WhenSlambookFound_ShouldReturnSlambookDetailsDTO()
        {
            // Arrange
            var createdDate = new DateOnly(2026, 9, 23);
            var slambook = await SeedSlambook(s =>
            {
                s.Title = "My Slambook";
                s.Description = "A slambook for my friends.";
                s.CreatedDate = createdDate;
            });

            // Act
            var result = await _repository.GetSlambookDetails(slambook.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Slambook details found.", result.Message);

            var details = Assert.IsType<SlambookDetailsDTO>(result.Data);
            Assert.Equal(slambook.Id, details.Id);
            Assert.Equal("My Slambook", details.Title);
            Assert.Equal("A slambook for my friends.", details.Description);
            Assert.Equal(createdDate, details.CreatedDate);
        }

        [Fact]
        public async Task GetSlambookDetails_WhenSlambookNotFound_ShouldReturnNullDataAndSuccessFalse()
        {
            // Act
            var result = await _repository.GetSlambookDetails(999, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal("Slambook details not found.", result.Message);
            Assert.Null(result.Data);
        }

        #endregion
    }
}
