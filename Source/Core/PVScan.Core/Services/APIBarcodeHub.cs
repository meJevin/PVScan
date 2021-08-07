using Microsoft.AspNetCore.SignalR.Client;
using PVScan.Core.Models;
using PVScan.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using PVScan.Core.Models.API;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using System.Net.Http;
using PVScan.Core;

namespace PVScan.Core.Services
{
    public class APIBarcodeHub : IAPIBarcodeHub
    {
        public event EventHandler<ScannedBarcodeRequest> OnScanned;
        public event EventHandler<DeletedBarcodeRequest> OnDeleted;
        public event EventHandler<UpdatedBarcodeRequest> OnUpdated;
        public event EventHandler<List<ScannedBarcodeRequest>> OnScannedMultiple;
        public event EventHandler<List<DeletedBarcodeRequest>> OnDeletedMultiple;
        public event EventHandler<List<UpdatedBarcodeRequest>> OnUpdatedMultple;

        private HubConnection connection;

        readonly IIdentityService IdentityService;

        public APIBarcodeHub(IIdentityService identityService)
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
                    .WithUrl(API.BaseAddress + API.BarcodesHub,
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

            connection.On<ScannedBarcodeRequest>("Scanned", (req) =>
            {
                OnScanned?.Invoke(this, req);
            });
            connection.On<DeletedBarcodeRequest>("Deleted", (req) =>
            {
                OnDeleted?.Invoke(this, req);
            });
            connection.On<UpdatedBarcodeRequest>("Updated", (req) =>
            {
                OnUpdated?.Invoke(this, req);
            });
            connection.On<List<ScannedBarcodeRequest>>("Scanned", (req) =>
            {
                OnScannedMultiple?.Invoke(this, req);
            });
            connection.On<List<DeletedBarcodeRequest>>("Deleted", (req) =>
            {
                OnDeletedMultiple?.Invoke(this, req);
            });
            connection.On<List<UpdatedBarcodeRequest>>("Updated", (req) =>
            {
                OnUpdatedMultple?.Invoke(this, req);
            });

            connection.Closed += Connection_Closed;
        }

        private async Task Connection_Closed(Exception arg)
        {
            await Task.Delay(2000);
            SetupConnection();
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

        public async Task Deleted(DeletedBarcodeRequest req)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            // Todo: this could be Invoke
            await connection.SendAsync("Deleted", req);
        }

        public async Task Scanned(ScannedBarcodeRequest req)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            // Todo: this could be Invoke
            await connection.SendAsync("Scanned", req);
        }

        public async Task Updated(UpdatedBarcodeRequest req)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            // Todo: this could be Invoke
            await connection.SendAsync("Updated", req);
        }

        public async Task ScannedMultiple(List<ScannedBarcodeRequest> reqs)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            await connection.SendAsync("ScannedMultiple", reqs);
        }

        public async Task DeletedMultiple(List<DeletedBarcodeRequest> reqs)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            await connection.SendAsync("DeletedMultple", reqs);
        }

        public async Task UpdatedMultiple(List<UpdatedBarcodeRequest> reqs)
        {
            if (connection.State != HubConnectionState.Connected)
            {
                return;
            }

            await connection.SendAsync("UpdatedMultiple", reqs);
        }
    }
}
