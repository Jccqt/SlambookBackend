using Microsoft.EntityFrameworkCore;
using SlambookBackend.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slambook.UnitTests.Helpers
{
    public static class DbContextHelper
    {
        public static AppDbContext GetInMemoryContext()
        {
            return GetInMemoryContext(QueryTrackingBehavior.TrackAll);
        }

        // Same isolated database, but with the query tracking behaviour chosen by the caller.
        // NoTracking hands the repository a detached entity, so its changes never reach the
        // change tracker and SaveChangesAsync reports zero affected rows
        public static AppDbContext GetInMemoryContext(QueryTrackingBehavior trackingBehavior)
        {
            // Using a new GUID ensures every test gets its own isolated database instance
            // This will prevents tests from interfering with each other when run parallel
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseQueryTrackingBehavior(trackingBehavior)
                .Options;

            var context = new AppDbContext(options);

            context.Database.EnsureCreated();

            return context;
        }
    }
}
