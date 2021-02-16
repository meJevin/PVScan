using Microsoft.EntityFrameworkCore;
using PVScan.Mobile.DAL;
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
        protected PVScanMobileDbContext DbContext;

        public TestBase()
        {
            MockForms.Init();
            Application.Current = new Application();

            var optionsBuilder = new DbContextOptionsBuilder<PVScanMobileDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            DbContext = new PVScanMobileDbContext(optionsBuilder.Options);
        }
    }
}
