using Microsoft.AspNetCore.SignalR.Client;
using RestND.MVVM.Model.Employees;    // Role, AppPermission
using RestND.MVVM.Model.Security;     // AuthContext
using RestND.MVVM.View;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel.Main;
using System;
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
        public static HubConnection DishTypeHub { get; private set; }

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

            // Auto-close windows if their permission is revoked
            AuthContext.PermissionsChanged += (sender, args) =>
            {
                CloseWindowsIfNoPermission(AppPermission.Inventory, typeof(ProductWindow));
                CloseWindowsIfNoPermission(AppPermission.Employees, typeof(EmployeesWindow), typeof(RolesWindow));
                CloseWindowsIfNoPermission(AppPermission.Dishes, typeof(DishWindow));
                CloseWindowsIfNoPermission(AppPermission.SoftDrinks, typeof(SoftDrinkWindow));
                CloseWindowsIfNoPermission(AppPermission.Orders, typeof(OrdersHistory));
                CloseWindowsIfNoPermission(AppPermission.Reports, typeof(ReportWindow));
                CloseWindowsIfNoPermission(AppPermission.Tables, typeof(MainWindow));
                CloseWindowsIfNoPermission(AppPermission.Other, typeof(DiscountWindow), typeof(DishTypeWindow), typeof(VatPopup));
            };

            var mainWindow = new MainWindow { DataContext = SharedMainVM };
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

            EmployeeHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/employeeHub")
                .WithAutomaticReconnect()
                .Build();

            DishTypeHub = new HubConnectionBuilder()
                .WithUrl("http://localhost:5027/dishtypeHub")
                .WithAutomaticReconnect()
                .Build();

            // Register BEFORE or AFTER StartAsync (both fine)
            RegisterEmployeeHubHandlers();

            await InventoryHub.StartAsync();
            await DishHub.StartAsync();
            await TableHub.StartAsync();
            await MainHub.StartAsync();
            await EmployeeHub.StartAsync();
            await DishTypeHub.StartAsync();
        }

        // Every client reacts to role changes from the server
        private void RegisterEmployeeHubHandlers()
        {
            // ReceiveRoleUpdate(Role role, string action)
            EmployeeHub.On<Role, string>("ReceiveRoleUpdate", (role, action) =>
            {
                var disp = Current?.Dispatcher;
                void apply() => AuthContext.ApplyRoleUpdate(role);

                if (disp != null && !disp.CheckAccess())
                    disp.Invoke(apply);
                else
                    apply();
            });
        }

        // Helper: if a permission is NOT present, close listed windows
        private static void CloseWindowsIfNoPermission(AppPermission perm, params Type[] windowTypes)
        {
            if (AuthContext.Has(perm)) return;

            var disp = Current?.Dispatcher;
            void closeAction()
            {
                foreach (Window w in Current.Windows)
                {
                    foreach (var t in windowTypes)
                    {
                        if (t.IsInstanceOfType(w))
                        {
                            try { w.Close(); } catch { /* ignore */ }
                        }
                    }
                }
            }

            if (disp != null && !disp.CheckAccess())
                disp.Invoke(closeAction);
            else
                closeAction();
        }
    }
}
