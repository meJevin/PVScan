using Microsoft.EntityFrameworkCore;
using PVScan.Domain.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.EntityFramework.Tests.Base
{
    public class EFTest : IAsyncLifetime
    {
        protected readonly PVScanDbContext _db;

        public EFTest()
        {
            _db = PVScanDbContextFactory.Create(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            });
        }

        public async Task DisposeAsync()
        {
        }

        public async Task InitializeAsync()
        {
            await SeedDatabase();
        }

        private async Task SeedDatabase()
        { 
        }
    }
}