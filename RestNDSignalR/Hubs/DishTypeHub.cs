using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using RestND.MVVM.Model;

namespace RestNDSignalR.Hubs
{
    public class DishTypeHub : Hub
    {
        public async Task NotifyDishTypeUpdate(DishType dishType, string action)
        {
            await Clients.All.SendAsync("ReceiveDishTypeUpdate", dishType, action);
        }
    }
}
