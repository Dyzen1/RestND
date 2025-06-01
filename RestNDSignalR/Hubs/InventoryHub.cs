using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model;

namespace RestNDSignalR.Hubs
{
    public class InventoryHub : Hub

    {
        public async Task NotifyInventoryUpdate(Inventory product, string action)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", product, action);
        }


    }
}
