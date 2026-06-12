using Microsoft.EntityFrameworkCore;
using SlambookBackend.Models;

namespace SlambookBackend.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }
    }
}
