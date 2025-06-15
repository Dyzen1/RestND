using Microsoft.AspNetCore.SignalR;

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

    }
}
