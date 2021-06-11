using PVScan.Core.Services.Interfaces;
using PVScan.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using Moq.Protected;
using System.Threading;
using System.Net.Http.Headers;
using PVScan.Core.Models;
using System.Linq;

namespace PVScan.Core.Tests.Services
{
    public class BarcodesRepositoryTests : TestBase
    {
        public BarcodesRepositoryTests()
        {
        }

        [Fact]
        public async Task Can_Save_Barcode()
        {
            // Arrange
            var sut = new BarcodesRepository(DbContext);

            // Act
            await sut.Save(new Barcode()
            {
                Text = "Test",
            });

            // Assert
            Assert.Equal(1, DbContext.Barcodes.Count());
            Assert.Equal("Test", DbContext.Barcodes.First().Text);
        }

        [Fact]
        public async Task Saved_Barcode_Has_GUID()
        {
            // Arrange
            var sut = new BarcodesRepository(DbContext);

            // Act
            await sut.Save(new Barcode()
            {
                Text = "Test",
            });

            // Assert
            Assert.Equal(1, DbContext.Barcodes.Count());

            var savedBarcode = DbContext.Barcodes.First();
            Assert.Equal("Test", savedBarcode.Text);
            Assert.NotNull(savedBarcode.GUID);
        }

        [Fact]
        public async Task Saved_Barcode_Has_Hash()
        {
            // Arrange
            var sut = new BarcodesRepository(DbContext);

            // Act
            await sut.Save(new Barcode()
            {
                Text = "Test",
            });

            // Assert
            Assert.Equal(1, DbContext.Barcodes.Count());

            var savedBarcode = DbContext.Barcodes.First();
            Assert.Equal("Test", savedBarcode.Text);
            Assert.NotNull(savedBarcode.Hash);
        }

        [Fact]
        public async Task Can_Get_All_Barcodes()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode2",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode3",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            var barcodes = (await sut.GetAll()).ToList();

            // Assert
            Assert.Equal(3, barcodes.Count());
            Assert.Equal("Barcode1", barcodes[0].Text);
            Assert.Equal("Barcode2", barcodes[1].Text);
            Assert.Equal("Barcode3", barcodes[2].Text);
        }

        [Fact]
        public async Task Can_Delete_Existing_Barcode()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode2",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode3",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            await sut.Delete(DbContext.Barcodes.First());

            // Assert
            var barcodes = DbContext.Barcodes.ToList();
            Assert.Equal(2, barcodes.Count());
            Assert.Equal("Barcode2", barcodes[0].Text);
            Assert.Equal("Barcode3", barcodes[1].Text);
        }

        [Fact]
        public async Task Can_Update_Existing_Barcode()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode2",
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode3",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            var barcodeToUpdate = DbContext.Barcodes.First();
            barcodeToUpdate.Text = "Barcode1_Updated";
            await sut.Update(barcodeToUpdate);

            // Assert
            var barcodes = DbContext.Barcodes.ToList();
            Assert.Equal(3, barcodes.Count());
            Assert.Equal("Barcode1_Updated", barcodes[0].Text);
            Assert.Equal("Barcode2", barcodes[1].Text);
            Assert.Equal("Barcode3", barcodes[2].Text);
        }

        [Fact]
        public async Task Updated_Barcode_Hash_Changed()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            var barcodeToUpdate = DbContext.Barcodes.First();
            var previousHash = barcodeToUpdate.Hash;

            barcodeToUpdate.Text = "Barcode1_Updated";
            await sut.Update(barcodeToUpdate);

            var newHash = barcodeToUpdate.Hash;

            // Assert
            Assert.NotEqual(previousHash, newHash);
        }

        [Fact]
        public async Task Can_Find_Existing_Barcode_By_GUID()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            var barcodeToFindByGUID = DbContext.Barcodes.First();
            var foundBarcode = await sut.FindByGUID(barcodeToFindByGUID.GUID);

            // Assert
            Assert.NotNull(foundBarcode);
            Assert.Equal(barcodeToFindByGUID, foundBarcode);
        }

        [Fact]
        public async Task Can_Not_Find_Non_Existing_Barcode_By_GUID()
        {
            // Arrange
            DbContext.Barcodes.Add(new Barcode()
            {
                Text = "Barcode1",
            });
            await DbContext.SaveChangesAsync();

            var sut = new BarcodesRepository(DbContext);

            // Act
            var foundBarcode = await sut.FindByGUID("Something");

            // Assert
            Assert.Null(foundBarcode);
        }
    }
}
