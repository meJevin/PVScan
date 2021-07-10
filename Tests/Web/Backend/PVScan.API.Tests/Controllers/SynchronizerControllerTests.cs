using Microsoft.AspNetCore.Mvc;
using PVScan.API.Controllers;
using PVScan.API.ViewModels.Synchronizer;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PVScan.API.Tests.Controllers
{
    public class SynchronizerControllerTests : TestBase
    {
        [Fact]
        public async Task Can_Identify_Barcodes_To_Add_Localy()
        {
            // Arrange
            SynchronizerController sut = new SynchronizerController(_context);
            _context.Barcodes.Add(new Barcode()
            {
                Id = 1,
                GUID = "guid1",
            });
            _context.Barcodes.Add(new Barcode()
            {
                Id = 2,
                GUID = "guid2",
            });
            _context.SaveChanges();

            var inputParams = new List<LocalBarcodeInfo>();
            inputParams.Add(new LocalBarcodeInfo()
            {
                LocalId = 22,
                GUID = "guid2"
            });

            // Act
            var result = await sut.Synchronize(new SynchronizeRequest() 
            {
                LocalBarcodeInfos = inputParams
            }) as ObjectResult;
            var resultObject = result.Value as SynchronizeResponse;

            // Assert
            Assert.Single(resultObject.ToAddLocaly);
            Assert.Equal("guid1", resultObject.ToAddLocaly.First().GUID);
            Assert.Equal(0, resultObject.ToAddLocaly.First().Id);
        }

        [Fact]
        public async Task Can_Identify_Barcodes_To_Update_Localy()
        {
            // Arrange
            SynchronizerController sut = new SynchronizerController(_context);
            _context.Barcodes.Add(new Barcode()
            {
                Id = 1,
                GUID = "guid1",
                LastUpdateTime = new DateTime(2000, 1, 1, 12, 1, 1),
            });
            _context.Barcodes.Add(new Barcode()
            {
                Id = 2,
                GUID = "guid2",
                Hash = "2",
                LastUpdateTime = new DateTime(2000, 1, 1, 12, 1, 1),
            });
            _context.SaveChanges();

            var inputParams = new List<LocalBarcodeInfo>();
            inputParams.Add(new LocalBarcodeInfo()
            {
                LocalId = 22,
                GUID = "guid2",
                Hash = "2change",
                LastTimeUpdated = new DateTime(2000, 1, 1, 11, 1, 1),
            });

            // Act
            var result = await sut.Synchronize(new SynchronizeRequest()
            {
                LocalBarcodeInfos = inputParams
            }) as ObjectResult;
            var resultObject = result.Value as SynchronizeResponse;

            // Assert
            Assert.Single(resultObject.ToUpdateLocaly);
            Assert.Equal("guid2", resultObject.ToUpdateLocaly.First().GUID);
            Assert.Equal(22, resultObject.ToUpdateLocaly.First().Id);
        }

        [Fact]
        public async Task Can_Identify_Barcodes_To_Send_To_Server()
        {
            // Arrange
            SynchronizerController sut = new SynchronizerController(_context);
            _context.Barcodes.Add(new Barcode()
            {
                GUID = "guid1",
            });
            _context.Barcodes.Add(new Barcode()
            {
                GUID = "guid2",
            });
            _context.SaveChanges();

            var inputParams = new List<LocalBarcodeInfo>();
            inputParams.Add(new LocalBarcodeInfo()
            {
                GUID = "guid2",
            });
            inputParams.Add(new LocalBarcodeInfo()
            {
                GUID = "guid3",
            });

            // Act
            var result = await sut.Synchronize(new SynchronizeRequest()
            {
                LocalBarcodeInfos = inputParams
            }) as ObjectResult;
            var resultObject = result.Value as SynchronizeResponse;

            // Assert
            Assert.Single(resultObject.ToAddToServer);
            Assert.Equal("guid3", resultObject.ToAddToServer.First());
        }

        [Fact]
        public async Task Can_Identify_Barcodes_To_Update_On_Server()
        {
            // Arrange
            SynchronizerController sut = new SynchronizerController(_context);
            _context.Barcodes.Add(new Barcode()
            {
                GUID = "guid1",
                Hash = "hash1",
                LastUpdateTime = new DateTime(2000, 1, 1, 12, 1, 1),
            });
            _context.Barcodes.Add(new Barcode()
            {
                GUID = "guid2",
                Hash = "hash2",
                LastUpdateTime = new DateTime(2000, 1, 1, 12, 1, 1),
            });
            _context.SaveChanges();

            var inputParams = new List<LocalBarcodeInfo>();
            inputParams.Add(new LocalBarcodeInfo()
            {
                GUID = "guid2",
                Hash = "hash2_changed",
                LastTimeUpdated = new DateTime(2000, 1, 1, 13, 1, 1),
            });

            // Act
            var result = await sut.Synchronize(new SynchronizeRequest()
            {
                LocalBarcodeInfos = inputParams
            }) as ObjectResult;
            var resultObject = result.Value as SynchronizeResponse;

            // Assert
            Assert.Single(resultObject.ToUpdateOnServer);
            Assert.Equal("guid2", resultObject.ToUpdateOnServer.First());
        }
    }
}
