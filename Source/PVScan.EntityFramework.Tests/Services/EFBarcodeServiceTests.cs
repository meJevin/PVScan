using PVScan.Domain.Models;
using PVScan.EntityFramework.Services;
using PVScan.EntityFramework.Tests.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.EntityFramework.Tests.Services
{
    public class EFBarcodeServiceTests : EFTest
    {
        [Fact]
        public async Task Can_Create_Barcode_Made_By_Existing_User()
        {
            // Arrange
            User u1 = new User() {
                Email = "test@mail.com",
                Username = "bob",
            };
            Barcode b1 = new Barcode() { ScannedBy = u1 };
            _db.Users.Add(u1);

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act
            var result = await target.Create(b1);

            // Assert
            Assert.Equal(result.ScannedBy.Email, u1.Email);
        }

        [Fact]
        public async Task Creating_Barcode_With_Nonexistent_User_Throws_Exception()
        {
            // Arrange
            User u1 = new User()
            {
                Email = "test@mail.com",
                Username = "bob",
            };
            Barcode b1 = new Barcode() { ScannedBy = u1 };

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(async () => {
                var result = await target.Create(b1);
            });
            Assert.Equal(0, _db.Barcodes.Count());
        }

        [Fact]
        public async Task Creating_Barcode_With_Null_User_Throws_Exception()
        {
            // Arrange
            User u1 = new User()
            {
                Email = "test@mail.com",
                Username = "bob",
            };
            Barcode b1 = new Barcode() { ScannedBy = null };

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(async () => {
                var result = await target.Create(b1);
            });
            Assert.Equal(0, _db.Barcodes.Count());
        }

    }
}
