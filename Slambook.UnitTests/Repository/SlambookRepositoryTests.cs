using Microsoft.EntityFrameworkCore;
using Slambook.UnitTests.DataGenerators;
using Slambook.UnitTests.Helpers;
using SlambookBackend.Context;
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
    }
}
