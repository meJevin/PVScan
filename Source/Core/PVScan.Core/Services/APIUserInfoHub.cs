using Microsoft.AspNetCore.SignalR.Client;
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
    // Todo: this is similar to Barcodes hub. Please rework
    public class APIUserInfoHub : IAPIUserInfoHub
    {
        public event EventHandler<GetUserInfoResponse> OnChanged;

        private HubConnection connection;

        readonly IIdentityService IdentityService;

        public APIUserInfoHub(IIdentityService identityService)
        {
            IdentityService = identityService;

            SetupConnection();
        }

        private void SetupConnection()
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithAutomaticReconnect(new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    })
                    .WithUrl(API.BaseAddress + API.UserInfoHub,
                        o =>
                        {
                            o.AccessTokenProvider = () => { return Task.FromResult(IdentityService.AccessToken); };
                            o.WebSocketConfiguration = conf =>
                            {
                                conf.RemoteCertificateValidationCallback = (m, c, ch, e) => { return true; };
                            };
                            o.HttpMessageHandlerFactory = factory => new HttpClientHandler
                            {
                                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                            };
                        })
                    .Build();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            connection.On<GetUserInfoResponse>("Changed", (req) =>
            {
                OnChanged?.Invoke(this, req);
            });

            connection.Closed += Connection_Closed;
        }

        private async Task Connection_Closed(Exception arg)
        {
            await Task.Delay(2000);
            SetupConnection();
        }

        public async Task Changed(GetUserInfoResponse req)
        {
            await connection.SendAsync("Changed", req);
        }

        public async Task Connect()
        {
            if (connection.State != HubConnectionState.Disconnected)
            {
                return;
            }

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Disconnect()
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            try
            {
                await connection.StopAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
