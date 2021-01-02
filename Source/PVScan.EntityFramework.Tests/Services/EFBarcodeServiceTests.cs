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
            _db.Add(u1);
            _db.SaveChanges();

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

        [Fact]
        public async Task Can_Get_User_Barcodes()
        {
            // Arrange
            User u1 = new User()
            {
                Email = "test@mail.com",
                Username = "bob",
            };
            _db.Add(u1);

            Barcode b1 = new Barcode() { ScannedBy = u1 };
            Barcode b2 = new Barcode() { ScannedBy = u1 };
            Barcode b3 = new Barcode() { ScannedBy = u1 };

            _db.Add(b1);
            _db.Add(b2);
            _db.Add(b3);
            _db.SaveChanges();

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            var result = await target.GetBarcodesForUser(u1);

            Assert.Equal(3, result.Count());
            Assert.All(result, (b) => { Assert.Equal(u1.Email, b.ScannedBy.Email); });
        }

        [Fact]
        public async Task Getting_Barcodes_For_Nonexistent_User_Returns_Nothing()
        {
            // Arrange
            User u1 = new User()
            {
                Email = "test@mail.com",
                Username = "bob",
            };

            Barcode b1 = new Barcode() { ScannedBy = u1 };
            Barcode b2 = new Barcode() { ScannedBy = u1 };
            Barcode b3 = new Barcode() { ScannedBy = u1 };

            _db.Add(b1);
            _db.Add(b2);
            _db.Add(b3);
            _db.Attach(u1); // Because we don't want our user to be put into DB
            _db.SaveChanges();

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            var result = await target.GetBarcodesForUser(u1);

            Assert.Empty(result);
        }
    }
}
