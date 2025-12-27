using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model.Tables;

namespace RestNDSignalR.Hubs
{
    public class MainHub: Hub
    {
        #region Main Update

        public async Task NotifyMainUpdate(string message, string action)
        {
            await Clients.All.SendAsync("ReceiveMainUpdate", message, action);
        }


        #endregion

        #region Table Update ✅
        public async Task NotifyTableUpdate(Table table, string action)
        {
            await Clients.All.SendAsync("ReceiveTableUpdate", table, action);
        }
        #endregion

    }
}
