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
            _db.Add(u1);
            _db.SaveChanges();

            Barcode b1 = new Barcode() { UserId = u1.Id };

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act
            var result = await target.Create(b1);

            // Assert
            Assert.Equal(result.UserId, u1.Id);
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
            Barcode b1 = new Barcode() { UserId = 666 };

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(async () => {
                var result = await target.Create(b1);
            });
            Assert.Equal(0, _db.Barcodes.Count());
        }

        [Fact]
        public async Task Can_Get_Barcodes_For_Existing_User()
        {
            // Arrange
            User u1 = new User()
            {
                Email = "test@mail.com",
                Username = "bob",
            };
            _db.Add(u1);
            _db.SaveChanges();

            Barcode b1 = new Barcode() { UserId = u1.Id };
            Barcode b2 = new Barcode() { UserId = u1.Id };
            Barcode b3 = new Barcode() { UserId = u1.Id };

            _db.Add(b1);
            _db.Add(b2);
            _db.Add(b3);
            _db.SaveChanges();

            EFBarcodeService target = new EFBarcodeService(_db);

            // Act + Assert
            var result = await target.GetBarcodesForUser(u1);

            Assert.Equal(3, result.Count());
            Assert.All(result, (b) => { Assert.Equal(u1.Id, b.UserId); });
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

            Barcode b1 = new Barcode() { UserId = u1.Id };
            Barcode b2 = new Barcode() { UserId = u1.Id };
            Barcode b3 = new Barcode() { UserId = u1.Id };

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
