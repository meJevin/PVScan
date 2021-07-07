using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PVScan.Core.Services.Interfaces
{
    public interface IHttpClientFactory
    {
        // Produce a regular HttpClient, nothing extra
        HttpClient Default();

        // Produce an HttpClient with bearer token attached to interact with API
        HttpClient ForAPI(string token);
    }
}
