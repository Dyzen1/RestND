using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestND.MVVM.Model.Tables;

namespace RestNDSignalR.Hubs
{
    public class TableHub : Hub
    {
        #region Table Update
        public async Task NotifyTableUpdate(Table table, string action)

        {

            await Clients.All.SendAsync("ReceiveTableUpdate", table, action);
        }
        #endregion



    }
}
