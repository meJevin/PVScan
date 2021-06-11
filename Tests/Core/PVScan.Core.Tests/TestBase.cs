using System;
using Microsoft.EntityFrameworkCore;
using PVScan.Core.DAL;

namespace PVScan.Core.Tests
{
    public class TestBase
    {
        protected PVScanDbContext DbContext;

        public TestBase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PVScanDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            DbContext = new PVScanDbContext(optionsBuilder.Options);
        }
    }
}
