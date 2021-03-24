using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Mobile.DAL;
using PVScan.Mobile.Models;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.Mobile.Tests.ViewModels
{
    public class HistoryPageViewModelTests : TestBase
    {
        public HistoryPageViewModelTests()
        {

        }

        [Fact]
        public void Can_Create_Empty()
        {
            // Arrange + Act
            HistoryPageViewModel vm = new HistoryPageViewModel(null, null, null);

            // Assert
            Assert.Empty(vm.Barcodes);
            Assert.Null(vm.CurrentFilter);
            Assert.False(vm.IsLoading);
            Assert.False(vm.IsRefresing);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_Without_Filter()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    Text = "B1",
                },
                new Barcode()
                {
                    Text = "B2",
                },
                new Barcode()
                {
                    Text = "B3",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());

            // Act
            await vm.LoadBarcodesFromDB();
            
            // Assert
            Assert.Equal(3, vm.Barcodes.Count);
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Filter_By_Barcode_Format()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Text = "B1",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Text = "B2",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.AZTEC,
                    Text = "B3",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.CurrentFilter = new Filter()
            {
                BarcodeFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE },
            };

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(2, vm.Barcodes.Count);
            Assert.All(vm.Barcodes, t => Assert.Equal(ZXing.BarcodeFormat.QR_CODE, t.Format));
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Filter_By_Date_Latest()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    ScanTime = DateTime.Now,
                    Text = "B1",
                },
                new Barcode()
                {
                    ScanTime = DateTime.Now.AddDays(-1),
                    Text = "B2",
                },
                new Barcode()
                {
                    ScanTime = DateTime.Now.AddDays(-2),
                    Text = "B3",
                },
                new Barcode()
                {
                    ScanTime = DateTime.Now.AddDays(-10),
                    Text = "B4",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.CurrentFilter = new Filter()
            {
                BarcodeFormats = Enumerable.Empty<ZXing.BarcodeFormat>(),
                LastType = LastTimeType.Week
            };

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(3, vm.Barcodes.Count);
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Filter_By_Date_Range()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    ScanTime = new DateTime(2000, 1, 1),
                    Text = "B1",
                },
                new Barcode()
                {
                    ScanTime = new DateTime(2003, 1, 1),
                    Text = "B2",
                },
                new Barcode()
                {
                    ScanTime = new DateTime(2005, 1, 1),
                    Text = "B3",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.CurrentFilter = new Filter()
            {
                BarcodeFormats = Enumerable.Empty<ZXing.BarcodeFormat>(),
                FromDate = new DateTime(2000, 1, 1),
                ToDate = new DateTime(2004, 1, 1),
            };

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(2, vm.Barcodes.Count);
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Filter_By_Date_Range_And_Format()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ScanTime = new DateTime(2000, 1, 1),
                    Text = "B1",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ScanTime = new DateTime(2003, 1, 1),
                    Text = "B2",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.AZTEC,
                    ScanTime = new DateTime(2005, 1, 1),
                    Text = "B3",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.CurrentFilter = new Filter()
            {
                BarcodeFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE },
                FromDate = new DateTime(2000, 1, 1),
                ToDate = new DateTime(2004, 1, 1),
            };

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(2, vm.Barcodes.Count);
            Assert.All(vm.Barcodes, t => Assert.Equal(ZXing.BarcodeFormat.QR_CODE, t.Format));
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Filter_By_Date_Latest_And_Format()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ScanTime = DateTime.Now,
                    Text = "B1",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ScanTime = DateTime.Now.AddDays(-1),
                    Text = "B2",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    ScanTime = DateTime.Now.AddDays(-2),
                    Text = "B3",
                },
                new Barcode()
                {
                    Format = ZXing.BarcodeFormat.AZTEC,
                    ScanTime = DateTime.Now.AddDays(-10),
                    Text = "B3",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.CurrentFilter = new Filter()
            {
                BarcodeFormats = new List<ZXing.BarcodeFormat> { ZXing.BarcodeFormat.QR_CODE },
                LastType = LastTimeType.Week,
            };

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(3, vm.Barcodes.Count);
            Assert.All(vm.Barcodes, t => Assert.Equal(ZXing.BarcodeFormat.QR_CODE, t.Format));
            Assert.False(vm.IsLoading);
        }

        [Fact]
        public async Task Can_Load_Barcodes_From_DB_With_Search()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    Text = "B2",
                },
                new Barcode()
                {
                    Text = "B2",
                },
                new Barcode()
                {
                    Text = "B3",
                },
                new Barcode()
                {
                    Text = "B4",
                },
            });
            DbContext.SaveChanges();

            var mockRepo = new BarcodesRepository(DbContext);
            HistoryPageViewModel vm = new HistoryPageViewModel(mockRepo, new BarcodesFilter());
            vm.Search = "B2";

            // Act
            await vm.LoadBarcodesFromDB();

            // Assert
            Assert.Equal(2, vm.Barcodes.Count);
            Assert.All(vm.Barcodes, t => Assert.Equal("B2", t.Text));
            Assert.False(vm.IsLoading);
        }
    }
}
