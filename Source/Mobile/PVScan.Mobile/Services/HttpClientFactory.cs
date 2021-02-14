using PVScan.Mobile.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PVScan.Mobile.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient ForAPI(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(API.BaseAddress);

            return client;
        }

        HttpClient IHttpClientFactory.Default()
        {
            return new HttpClient();
        }
    }

    // Debug stuff
    // This factory produces clients which work with debug HTTPS certificate
    public class DebugCertHttpClientFactory : IHttpClientFactory
    {
        private static HttpClientHandler DebugCertHandler;

        static DebugCertHttpClientFactory()
        {
            DebugCertHandler = new HttpClientHandler();
            DebugCertHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
        }

        public HttpClient Default()
        {
            return new HttpClient(DebugCertHandler);
        }

        public HttpClient ForAPI(string token)
        {
            HttpClient client = new HttpClient(DebugCertHandler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(API.BaseAddress);

            return client;
        }
    }
}
