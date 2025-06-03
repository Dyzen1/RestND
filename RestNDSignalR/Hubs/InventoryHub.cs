using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model;

namespace RestNDSignalR.Hubs
{
    public class InventoryHub : Hub
    {
        #region Inventory Update
        public async Task NotifyInventoryUpdate(Inventory inventory)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", inventory);
        }

        #endregion
    }
}