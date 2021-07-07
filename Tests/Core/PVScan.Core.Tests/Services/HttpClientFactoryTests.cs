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

namespace PVScan.Core.Tests.Services
{
    public class HttpClientFactoryTests : TestBase
    {
        public HttpClientFactoryTests()
        {
        }

        [Fact]
        public void Factory_ForAPI_Has_Correct_Base_Address()
        {
            // Arrange
            var sut = new HttpClientFactory();

            // Act
            var result = sut.ForAPI("some_token");

            // Assert
            Assert.Equal(API.BaseAddress, result.BaseAddress.AbsoluteUri);
        }

        [Fact]
        public void Factory_ForAPI_Has_Bearer_Authentication_Set()
        {
            // Arrange
            var sut = new HttpClientFactory();

            // Act
            var result = sut.ForAPI("some_token");

            // Assert
            var authHeader = result.DefaultRequestHeaders.Authorization;
            Assert.Equal("Bearer", authHeader.Scheme);
            Assert.Equal("some_token", authHeader.Parameter);
        }
    }
}
