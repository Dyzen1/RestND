using Microsoft.AspNetCore.SignalR.Client;
<<<<<<< HEAD
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
=======
using RestND.MVVM.View.Windows;
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
using System.Threading.Tasks;
using System.Windows;

namespace RestND
{
    public partial class App : Application
    {
        public static HubConnection HubConnection { get; private set; }
<<<<<<< HEAD
=======

>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
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

            // Show the main window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private async Task InitializeSignalR()
        {
            HubConnection = new HubConnectionBuilder()
<<<<<<< HEAD
                .WithUrl("http://localhost:5047/inventoryHub")
=======
                .WithUrl("http://localhost:5027/inventoryHub")
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
                .WithAutomaticReconnect()
                .Build();

            await HubConnection.StartAsync();
        }
    }
}
