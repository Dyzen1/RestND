using Microsoft.AspNetCore.SignalR.Client;
using Mysqlx.Crud;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Data
{
    public class SignalRServices
    {
        #region Properties
        private readonly HubConnection _connection;
        #endregion

        #region Events
        public event Action<Inventory> InventoryUpdated;
        #endregion

        #region Constructor
        public SignalRServices(HubConnection connection)
        {
            _connection = connection;
            _connection.On<Inventory>("ReceiveInventoryUpdate", (inventory) =>
            {
                InventoryUpdated?.Invoke(inventory);
            });
        }

        #endregion

        #region Methods
        public async Task StartAsync()
        {
            await _connection.StartAsync();
        }

        public async Task SendInventoryUpdateAsync(Inventory inventory)
        {
            await _connection.SendAsync("NotifyInventoryUpdated", inventory);
        }

        #endregion
    }
}

