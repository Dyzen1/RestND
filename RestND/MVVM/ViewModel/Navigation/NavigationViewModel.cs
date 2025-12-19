using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.Model.Employees;    // AppPermission
using RestND.MVVM.Model.Security;     // AuthContext
using RestND.MVVM.View;
using RestND.MVVM.View.Windows;
using System;
using System.Linq;
using System.Windows;

namespace RestND.MVVM.ViewModel.Navigation
{
    public partial class NavigationViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        private string currentView = string.Empty; // name of the current window

        private readonly EventHandler _permHandler;

        // Subscribe to live permission changes so bound buttons update (IsEnabled/Visibility)
        public NavigationViewModel()
        {
            _permHandler = (s, e) =>
            {
                OnPropertyChanged(nameof(CanOpenInventory));
                OnPropertyChanged(nameof(CanOpenManageRoles));
                OnPropertyChanged(nameof(CanOpenManageDiscounts));
                OnPropertyChanged(nameof(CanOpenManageDishTypes));
                OnPropertyChanged(nameof(CanOpenDishes));
                OnPropertyChanged(nameof(CanOpenReports));
                OnPropertyChanged(nameof(CanOpenEmployees));
                OnPropertyChanged(nameof(CanOpenOrders));
                OnPropertyChanged(nameof(CanOpenMain));
                OnPropertyChanged(nameof(CanOpenSoftDrinks));
                OnPropertyChanged(nameof(CanOpenVat));
            };

            AuthContext.PermissionsChanged += _permHandler;
        }

        [RelayCommand]
        private void Navigate(string destination)
        {
            if (destination == "ManageVat")
            {
                // gate VAT popup too
                if (!AuthContext.Has(AppPermission.Other))
                {
                    MessageBox.Show("You don't have permission to open this page.");
                    return;
                }

                OpenVatDialog();
                return;
            }

            // map destinations to required flags
            AppPermission required = destination switch
            {
                "Inventory" => AppPermission.Inventory,
                "ManageRoles" => AppPermission.Employees,   // choose Other if that's your policy
                "ManageDiscounts" => AppPermission.Other,
                "ManageDishTypes" => AppPermission.Other,
                "Dishes" => AppPermission.Dishes,
                "Reports" => AppPermission.Reports,
                "Employees" => AppPermission.Employees,
                "Orders" => AppPermission.Orders,
                "Main" => AppPermission.Tables,
                "SoftDrinks" => AppPermission.SoftDrinks,
                _ => AppPermission.None
            };

            if (required != AppPermission.None && !AuthContext.Has(required))
            {
                MessageBox.Show("You don't have permission to open this page.");
                return;
            }

            // open window as before
            Window? next = destination switch
            {
                "Inventory" => new ProductWindow(),
                "ManageRoles" => new RolesWindow(),
                "ManageDiscounts" => new DiscountWindow(),
                "ManageDishTypes" => new DishTypeWindow(),
                "Dishes" => new DishWindow(),
                "Reports" => new ReportWindow(),
                "Employees" => new EmployeesWindow(),
                "Orders" => new OrdersHistory(),
                "Main" => new MainWindow(),
                "SoftDrinks" => new SoftDrinkWindow(),
                _ => null
            };
            if (next == null) return;

            var app = Application.Current;
            var old = app.MainWindow;

            next.WindowState = WindowState.Maximized;
            app.MainWindow = next;
            next.Show();
            old?.Hide();
        }

        private void OpenVatDialog()
        {
            var rootMain = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            var dlg = new VatPopup
            {
                Owner = rootMain ?? Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (dlg.Owner != null) dlg.Owner.Opacity = 0.4;
            dlg.ShowDialog();
            if (dlg.Owner != null) dlg.Owner.Opacity = 1.0;
        }

        // functions for each authorization.
        public bool CanOpenInventory => AuthContext.Has(AppPermission.Inventory);
        public bool CanOpenManageRoles => AuthContext.Has(AppPermission.Employees);   // or Other
        public bool CanOpenManageDiscounts => AuthContext.Has(AppPermission.Other);
        public bool CanOpenManageDishTypes => AuthContext.Has(AppPermission.Other);
        public bool CanOpenDishes => AuthContext.Has(AppPermission.Dishes);
        public bool CanOpenReports => AuthContext.Has(AppPermission.Reports);
        public bool CanOpenEmployees => AuthContext.Has(AppPermission.Employees);
        public bool CanOpenOrders => AuthContext.Has(AppPermission.Orders);
        public bool CanOpenMain => AuthContext.Has(AppPermission.Tables);
        public bool CanOpenSoftDrinks => AuthContext.Has(AppPermission.SoftDrinks);
        public bool CanOpenVat => AuthContext.Has(AppPermission.Other);

        // Optional: clean-up
        public void Dispose()
        {
            AuthContext.PermissionsChanged -= _permHandler;
        }
    }
}
