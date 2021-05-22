using Microsoft.EntityFrameworkCore;
using Moq;
using PVScan.Core.DAL;
using PVScan.Core.Models;
using PVScan.Core.Services;
using PVScan.Mobile.Services;
using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.Tests.Services.Mocks;
using PVScan.Mobile.ViewModels;
using PVScan.Mobile.ViewModels.Messages.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xunit;
using ZXing;

namespace PVScan.Mobile.Tests.ViewModels
{
    public class FilterPageViewModelTests : TestBase
    {
        [Fact]
        public void Can_Initialize_With_Date_Ranges_From_Repository()
        {
            // Arrange
            DbContext.Barcodes.AddRange(new List<Barcode>()
            {
                new Barcode()
                {
                    ScanTime = new DateTime(2000, 1, 1),
                },
                new Barcode()
                {
                    ScanTime = new DateTime(2003, 1, 1),
                },
                new Barcode()
                {
                    ScanTime = new DateTime(2005, 1, 1),
                },
            });
            DbContext.SaveChanges();
            BarcodesRepository repo = new BarcodesRepository(DbContext);

            // Act
            FilterPageViewModel vm = new FilterPageViewModel(repo);

            // Assert
            Assert.Equal(new DateTime(2000, 1, 1), vm.FromDate);
            Assert.Equal(new DateTime(2005, 1, 1), vm.ToDate);
        }

        [Fact]
        public void Can_Initialize_With_Barcode_Formats()
        {
            // Arrange
            BarcodesRepository repo = new BarcodesRepository(DbContext);
            var expectedFormats = Enum.GetValues(typeof(BarcodeFormat))
                    .OfType<BarcodeFormat>()
                    .ToList();

            // Act
            FilterPageViewModel vm = new FilterPageViewModel(repo);
            var formats = vm.AvailableBarcodeFormats
                .Select(f => f.Format)
                .ToList();

            // Assert
            Assert.Equal(expectedFormats, formats);
        }

        [Fact]
        public void Can_Initialize_With_Last_Time_Spans()
        {
            // Arrange
            BarcodesRepository repo = new BarcodesRepository(DbContext);
            var expectedSpans = Enum.GetValues(typeof(LastTimeType))
                    .OfType<LastTimeType>()
                    .ToList();

            // Act
            FilterPageViewModel vm = new FilterPageViewModel(repo);
            var spans = vm.AvailableLastTimeSpans
                .Select(s => s.Type)
                .ToList();

            // Assert
            Assert.Equal(expectedSpans, spans);
        }

        // This unit tests breaks other unit tests in history page because of MessagingCenter presumably
        //[Fact]
        //public void Can_Apply_Filter()
        //{
        //    // Arrange
        //    BarcodesRepository repo = new BarcodesRepository(DbContext);
        //    FilterPageViewModel vm = new FilterPageViewModel(repo);

        //    Filter gotFilter = null;

        //    MessagingCenter.Subscribe(this, nameof(FilterAppliedMessage),
        //        async (FilterPageViewModel vm, FilterAppliedMessage args) =>
        //        {
        //            gotFilter = args.NewFilter;
        //        });

        //    // Date filter type range
        //    vm.DateFilterTypeIndex = 1;
        //    vm.FromDate = new DateTime(2000, 1, 1);
        //    vm.ToDate = new DateTime(2005, 1, 1);

        //    // Act
        //    vm.ApplyFilterCommand.Execute(null);

        //    // Assert
        //    Assert.NotNull(gotFilter);
        //    Assert.Equal(new DateTime(2000, 1, 1), gotFilter.FromDate);
        //    Assert.Equal(new DateTime(2005, 1, 2), gotFilter.ToDate);
        //}
    }
}
