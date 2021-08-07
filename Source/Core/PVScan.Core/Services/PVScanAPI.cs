using Newtonsoft.Json;
using PVScan.Core.Models.API;
using PVScan.Core.Services.Interfaces;
using PVScan.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PVScan.Core.Services
{
    public class PVScanAPI : IPVScanAPI
    {
        readonly IIdentityService IdentityService;
        readonly IHttpClientFactory HttpClientFactory;

        public PVScanAPI(IIdentityService identityService, IHttpClientFactory httpClientFactory)
        {
            IdentityService = identityService;
            HttpClientFactory = httpClientFactory;
        }

        public async Task<ChangeUserInfoResponse> ChangeUserInfo(ChangeUserInfoRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                var client = HttpClientFactory.ForAPI(IdentityService.AccessToken);
                var contentToSend = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                var apiResponse = await client
                    .PostAsync("api/v1/users/change", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ChangeUserInfoResponse>(responseText);

                return responseObject;
            }
            catch
            {
                return null;
            }
        }

        public async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var apiResponse = await httpClient
                    .GetAsync("api/v1/users/current")
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<GetUserInfoResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<ScannedBarcodeResponse> ScannedBarcode(ScannedBarcodeRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/scanned", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ScannedBarcodeResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<UpdatedBarcodeResponse> UpdatedBarcode(UpdatedBarcodeRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/updated", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<UpdatedBarcodeResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<DeletedBarcodeRequest> DeletedBarcode(DeletedBarcodeRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/deleted", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<DeletedBarcodeRequest>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<SynchronizeResponse> Synchronize(SynchronizeRequest req)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/synchronizer/synchronize", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<SynchronizeResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<ScannedBarcodeResponse> ScannedBarcodeMultiple(List<ScannedBarcodeRequest> reqs)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(reqs), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/scanned-multiple", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<ScannedBarcodeResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<UpdatedBarcodeResponse> UpdatedBarcodeMultiple(List<UpdatedBarcodeRequest> reqs)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(reqs), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/updated-multiple", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<UpdatedBarcodeResponse>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }

        public async Task<DeletedBarcodeRequest> DeletedBarcodeMultiple(List<DeletedBarcodeRequest> reqs)
        {
            if (IdentityService.AccessToken == null)
            {
                return null;
            }

            try
            {
                HttpClient httpClient = HttpClientFactory.ForAPI(IdentityService.AccessToken);

                var contentToSend = new StringContent(JsonConvert.SerializeObject(reqs), Encoding.UTF8, "application/json");

                var apiResponse = await httpClient
                    .PostAsync("api/v1/barcodes/deleted-multiple", contentToSend)
                    .WithTimeout(DataAccess.WebRequestTimeout);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    // Could also throw an exception :)
                    return null;
                }

                var responseText = await apiResponse.Content.ReadAsStringAsync();
                var responseObject = JsonConvert.DeserializeObject<DeletedBarcodeRequest>(responseText);

                return responseObject;
            }
            catch
            {
                // Could also throw a meaningful exception :)
                return null;
            }
        }
    }
}
