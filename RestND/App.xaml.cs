using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND
{
    public partial class App : Application
    {
        public static HubConnection InventoryHub { get; private set; }
        public static HubConnection DishHub { get; private set; }
        public static HubConnection TableHub { get; private set; }
        public static HubConnection MainHub { get; private set; }
        public static HubConnection EmployeeHub { get; private set; }
        public static HubConnection DishTypeHub {  get; private set; }

        public static MainWindowViewModel SharedMainVM { get; private set; }


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
            SharedMainVM = new MainWindowViewModel();

            var mainWindow = new MainWindow
            {
                DataContext = SharedMainVM
            };
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

            TableHub = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5027/tableHub")
                    .WithAutomaticReconnect()
                    .Build();


            MainHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/mainHub")
                .WithAutomaticReconnect()
                .Build();

            EmployeeHub= new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/employeeHub")
                .WithAutomaticReconnect()
                .Build();


            DishTypeHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/dishtypeHub")
                .WithAutomaticReconnect()
                .Build();


            await InventoryHub.StartAsync();
            await DishHub.StartAsync();
            await TableHub.StartAsync();
            await MainHub.StartAsync();
            await EmployeeHub.StartAsync();
            await DishTypeHub.StartAsync();
        }
    }
}
