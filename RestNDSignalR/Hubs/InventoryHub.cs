using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model;

namespace RestNDSignalR.Hubs
{
    public class InventoryHub : Hub
<<<<<<< HEAD
    {
        #region Inventory Update
        public async Task NotifyInventoryUpdate(Inventory inventory)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", inventory);
        }

        #endregion
    }
}
=======

    {
        public async Task NotifyInventoryUpdate(Inventory product, string action)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", product, action);
        }


    }
}
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
