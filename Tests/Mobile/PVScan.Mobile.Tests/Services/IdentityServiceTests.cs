using PVScan.Mobile.Services.Interfaces;
using PVScan.Mobile.Tests.Services.Mocks;
using PVScan.Mobile.Services;
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

namespace PVScan.Mobile.Tests.Services
{
    public class IdentityServiceTests : TestBase
    {
        private readonly string ValidToken = "VALID_ACCESS_TOKEN";

        public IdentityServiceTests()
        {
        }

        [Fact]
        public async Task Can_Initialize_With_Valid_Access_Token_In_Storage()
        {
            // Arrange
            var KVPMock = new InMemoryKVP();
            var factoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>();

            // Set token to 'valid' token
            KVPMock.Set(StorageKeys.AccessToken, ValidToken);

            string storageAccessToken = KVPMock.Get(StorageKeys.AccessToken, null);

            // We check the validity of token by making a request to current user endpoint
            factoryMock.Setup(m => m.ForAPI(It.Is<string>(s => s == storageAccessToken))).Returns(() => 
            {
                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(b => b.Headers.Authorization.Parameter == ValidToken),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage() 
                    { 
                        StatusCode = System.Net.HttpStatusCode.OK 
                    });

                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(b => b.Headers.Authorization.Parameter != ValidToken),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.Unauthorized
                    });

                var client = new HttpClient(handlerMock.Object);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storageAccessToken);
                client.BaseAddress = new Uri(API.BaseAddress);

                return client;
            });

            var identityService = new IdentityService(KVPMock, factoryMock.Object);

            // Act
            await identityService.Initialize();

            // Assert
            Assert.Equal(identityService.AccessToken, ValidToken);
        }

        [Fact]
        public async Task Can_Initialize_With_Invalid_Access_Token_In_Storage()
        {
            // Arrange
            var KVPMock = new InMemoryKVP();
            var factoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>();

            // Set token to 'invalid' token
            KVPMock.Set(StorageKeys.AccessToken, Guid.NewGuid().ToString());

            string storageAccessToken = KVPMock.Get(StorageKeys.AccessToken, null);

            // If token is invalid, access token is set to null in storage and in service
            factoryMock.Setup(m => m.ForAPI(It.Is<string>(s => s == storageAccessToken))).Returns(() =>
            {
                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(b => b.Headers.Authorization.Parameter == ValidToken),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK
                    });

                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(b => b.Headers.Authorization.Parameter != ValidToken),
                    ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(new HttpResponseMessage()
                    {
                        StatusCode = System.Net.HttpStatusCode.Unauthorized
                    });

                var client = new HttpClient(handlerMock.Object);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storageAccessToken);
                client.BaseAddress = new Uri(API.BaseAddress);

                return client;
            });

            var identityService = new IdentityService(KVPMock, factoryMock.Object);

            // Act
            await identityService.Initialize();

            // Assert
            Assert.Null(identityService.AccessToken);
            Assert.Null(KVPMock.Get(StorageKeys.AccessToken, null));
        }
    }
}
