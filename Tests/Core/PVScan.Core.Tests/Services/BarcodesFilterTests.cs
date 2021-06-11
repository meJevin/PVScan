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
    public class BarcodesFilterTests : TestBase
    {
        public BarcodesFilterTests()
        {
            FillUpTestBarcodes();
        }

        [Fact]
        public void Default_Filter_Is_Empty_Filter()
        {
            // Arrange & Act
            var defaultFilter = Filter.Default();

            // Assert
            Assert.Empty(defaultFilter.BarcodeFormats);
            Assert.Null(defaultFilter.FromDate);
            Assert.Null(defaultFilter.ToDate);
            Assert.Null(defaultFilter.LastType);
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Date_Range()
        {
            // Arrange
            var fromDate = new DateTime(2010, 1, 1);
            var toDate = new DateTime(2010, 1, 3);

            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.FromDate = fromDate;
            filter.ToDate = toDate;

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= fromDate && b.ScanTime <= toDate);
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Last_Date_Type_Day()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Day;

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= DateTime.Today &&
                    b.ScanTime <= DateTime.Today.AddDays(1));
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Last_Date_Type_Week()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Week;

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= DateTime.Today.AddDays(-7) &&
                    b.ScanTime <= DateTime.Today.AddDays(1));
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Last_Date_Type_Month()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Month;

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= DateTime.Today.AddMonths(-1) &&
                    b.ScanTime <= DateTime.Today.AddDays(1));
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Last_Date_Type_Year()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Year;

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= DateTime.Today.AddYears(-1) &&
                    b.ScanTime <= DateTime.Today.AddDays(1));
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Formats_Single()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.Format == ZXing.BarcodeFormat.QR_CODE);
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Formats_Multiple()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.QR_CODE);
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.PDF_417);

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.Format == ZXing.BarcodeFormat.QR_CODE ||
                    b.Format == ZXing.BarcodeFormat.PDF_417);
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Formats_And_Date_Range()
        {
            // Arrange
            var fromDate = new DateTime(2010, 1, 1);
            var toDate = new DateTime(2010, 1, 3);

            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.FromDate = fromDate;
            filter.ToDate = toDate;
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= fromDate && b.ScanTime <= toDate &&
                    b.Format == ZXing.BarcodeFormat.QR_CODE);
            });
        }

        [Fact]
        public void Can_Filter_Barcodes_By_Formats_And_Last_Date_Type()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Day;
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= DateTime.Today &&
                    b.ScanTime <= DateTime.Today.AddDays(1) &&
                    b.Format == ZXing.BarcodeFormat.QR_CODE);
            });
        }

        [Fact]
        public void Empty_Filter_Does_Not_Filter()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.Equal(mockBarcodes, filteredBarcodes);
        }

        [Fact]
        public void Empty_Filter_Date_Range_And_Last_Date_Type_Does_Not_Filter_By_Date()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.BarcodeFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            var minDate = mockBarcodes.Min(b => b.ScanTime);
            var maxDate = mockBarcodes.Max(b => b.ScanTime);

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            Assert.All(filteredBarcodes, (b) =>
            {
                Assert.True(b.ScanTime >= minDate && b.ScanTime <= maxDate);
            });
        }

        [Fact]
        public void Empty_Filter_Barcode_Formats_Does_Not_Filter_By_Format()
        {
            // Arrange
            var mockBarcodes = DbContext.Barcodes.ToList();
            var sut = new BarcodesFilter();
            var filter = Filter.Default();
            filter.LastType = LastTimeType.Day;

            var existingFormats = mockBarcodes.Select(b => b.Format).Distinct();

            // Act
            var filteredBarcodes = sut.Filter(mockBarcodes, filter);

            // Assert
            var filteredFormats = filteredBarcodes.Select(b => b.Format).Distinct();
            Assert.Equal(existingFormats, filteredFormats);
        }

        private void FillUpTestBarcodes()
        {
            // For mock barcodes we add 3 barcodes for each time category
            // Each time category has different barcode formats

            // Today
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(2),
                Format = ZXing.BarcodeFormat.QR_CODE,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(3),
                Format = ZXing.BarcodeFormat.AZTEC,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddHours(4),
                Format = ZXing.BarcodeFormat.PDF_417,
            });

            // This week
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddDays(-7).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
            });

            // This month
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddMonths(-1).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
            });

            // This year
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(1),
                Format = ZXing.BarcodeFormat.QR_CODE,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(2),
                Format = ZXing.BarcodeFormat.AZTEC,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = DateTime.Today.AddYears(-1).AddDays(3),
                Format = ZXing.BarcodeFormat.PDF_417,
            });

            // Some time ago
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 1),
                Format = ZXing.BarcodeFormat.QR_CODE,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 2),
                Format = ZXing.BarcodeFormat.AZTEC,
            });
            DbContext.Barcodes.Add(new Barcode()
            {
                ScanTime = new DateTime(2010, 1, 3),
                Format = ZXing.BarcodeFormat.PDF_417,
            });

            // Save changes
            DbContext.SaveChanges();
        }
    }
}
