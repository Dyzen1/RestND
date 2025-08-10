using Microsoft.AspNetCore.SignalR;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using System.Threading.Tasks;

namespace RestNDSignalR.Hubs
{
    public class DishHub : Hub
    {
        public async Task NotifyDishUpdate(Dish dish, string action)
        {
            await Clients.All.SendAsync("ReceiveDishUpdate", dish, action);
        }
    }
}