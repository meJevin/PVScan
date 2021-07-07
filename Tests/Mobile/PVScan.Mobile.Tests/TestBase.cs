using Microsoft.EntityFrameworkCore;
using PVScan.Core.DAL;
using PVScan.Core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Mocks;

namespace PVScan.Mobile.Tests
{
    public class TestBase
    {
        protected PVScanDbContext DbContext;

        public TestBase()
        {
            MockForms.Init();
            Application.Current = new Application();

            var optionsBuilder = new DbContextOptionsBuilder<PVScanDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            DbContext = new PVScanDbContext(optionsBuilder.Options);
        }
    }
}
