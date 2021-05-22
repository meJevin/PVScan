using Microsoft.AspNetCore.SignalR;
using PVScan.API.ViewModels.Barcodes;
using PVScan.API.ViewModels.Users;
using PVScan.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PVScan.API.Hubs
{
    public class UserInfoHub : Hub
    {
        // Current user info has changed, let's notify all users!
        public async Task Changed(CurrentResponse res)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await Clients.Group(Context.UserIdentifier).SendAsync("Changed", res);
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
