using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using RestND.MVVM.View.Windows;
using System.Threading.Tasks;
using System.Windows;

namespace RestND
{
    public partial class App : Application
    {
        public static HubConnection InventoryHub { get; private set; }
        public static HubConnection DishHub { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeSignalR().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    MessageBox.Show("Failed to start SignalR: " + task.Exception.InnerException?.Message);
                }
            });

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private async Task InitializeSignalR()
        {
            InventoryHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/inventoryHub")
                .WithAutomaticReconnect()
                .Build();

            DishHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/dishHub")
                .WithAutomaticReconnect()
                .Build();

            await InventoryHub.StartAsync();
            await DishHub.StartAsync();
        }
    }
}
