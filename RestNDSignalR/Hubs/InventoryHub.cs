using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model;

namespace RestNDSignalR.Hubs
{
    public class InventoryHub : Hub
    {
        #region Inventory Update
        public async Task NotifyInventoryUpdate(Inventory inventory, string action)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", inventory, action);
        }

        #endregion
    }
}
