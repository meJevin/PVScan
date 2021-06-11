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
    public class BarcodeSorterTests : TestBase
    {
        public BarcodeSorterTests()
        {
            FillUpTestBarcodes();
        }

        [Fact]
        public void Default_Sorting_Is_Date_Descending()
        {
            // Arrange & Act
            var defaultSotring = Sorting.Default();

            Assert.Equal(SortingField.Date, defaultSotring.Field);
            Assert.True(defaultSotring.Descending);
        }

        [Fact]
        public async Task Can_Sort_By_Date_Descending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Date;
            sorting.Descending = true;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderByDescending(b => b.ScanTime).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }

        [Fact]
        public async Task Can_Sort_By_Date_Ascending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Date;
            sorting.Descending = false;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderBy(b => b.ScanTime).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }


        [Fact]
        public async Task Can_Sort_By_Text_Descending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Text;
            sorting.Descending = true;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderByDescending(b => b.Text).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }

        [Fact]
        public async Task Can_Sort_By_Text_Ascending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Text;
            sorting.Descending = false;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderBy(b => b.Text).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }


        [Fact]
        public async Task Can_Sort_By_Format_Descending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Format;
            sorting.Descending = true;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderByDescending(b => b.Format).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }

        [Fact]
        public async Task Can_Sort_By_Format_Ascending()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodeSorter();

            var sorting = Sorting.Default();
            sorting.Field = SortingField.Format;
            sorting.Descending = false;

            // Act
            var resultBarcodes = (await sut.Sort(mockBarcodes, sorting)).ToList();
            var sortedBarcodes = mockBarcodes.OrderBy(b => b.Format).ToList();

            Assert.Equal(sortedBarcodes, resultBarcodes);
        }

        private void FillUpTestBarcodes()
        {
            // For mock barcodes we add 3 barcodes for each time category
            // Each time category has different barcode formats

            List<Barcode> barcodesToAdd = new List<Barcode>();

            // Today
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(2),
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Test1",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(3),
                Format = ZXing.BarcodeFormat.AZTEC,
                Text = "Test2",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(4),
                Format = ZXing.BarcodeFormat.PDF_417,
                Text = "Test3",
            });

            // This week
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Test4",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
                Text = "Test5",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
                Text = "Test6",
            });

            // This month
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Test7",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
                Text = "Test8",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
                Text = "Test9",
            });

            // This year
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Test10",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
                Text = "Test11",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
                Text = "Test12",
            });

            // Some time ago
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 1),
                Format = ZXing.BarcodeFormat.QR_CODE,
                Text = "Test13",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 2),
                Format = ZXing.BarcodeFormat.AZTEC,
                Text = "Test14",
            });
            barcodesToAdd.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 3),
                Format = ZXing.BarcodeFormat.PDF_417,
                Text = "Test15",
            });

            // Shuffle
            barcodesToAdd = barcodesToAdd.OrderBy(x => Guid.NewGuid()).ToList();

            foreach (var b in barcodesToAdd)
            {
                DbContext.Barcodes.Add(b);
            }

            // Save changes
            DbContext.SaveChanges();
        }
    }
}
