using Microsoft.AspNetCore.SignalR.Client;
using RestND.MVVM.Model;
using System;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class SignalRService
    {
        private readonly HubConnection _connection;

        public event Action<Inventory> InventoryUpdated;

        public SignalRService(HubConnection connection)
        {
            _connection = connection;

            
            _connection.On<Inventory>("ReceiveInventoryUpdate", (inventory) =>
            {
                InventoryUpdated?.Invoke(inventory);
            });
        }

        public async Task StartAsync()
        {
           
                await _connection.StartAsync();
            
        }

        public async Task SendInventoryUpdateAsync(Inventory inventory)
        {
          
                await _connection.SendAsync("NotifyInventoryUpdated", inventory);
           
        }
    }
}
