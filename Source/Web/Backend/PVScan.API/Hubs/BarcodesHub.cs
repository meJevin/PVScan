using Microsoft.AspNetCore.SignalR;
using PVScan.Domain.DTO.Barcodes;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.Hubs
{
    public class BarcodesHub : Hub
    {
        public async Task Scanned(ScannedRequest req)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await Clients.Group(Context.UserIdentifier).SendAsync("Scanned", req);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        }

        public async Task Deleted(DeletedRequest req)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await Clients.Group(Context.UserIdentifier).SendAsync("Deleted", req);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        }

        public async Task Updated(UpdatedRequest req)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await Clients.Group(Context.UserIdentifier).SendAsync("Updated", req);
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
