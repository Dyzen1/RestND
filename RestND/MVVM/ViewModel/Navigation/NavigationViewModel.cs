using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.View;
using RestND.MVVM.View.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel.Navigation
{
    public partial class NavigationViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentView = string.Empty; //name of the current window

        [RelayCommand]
        private void Navigate(string destination)
        {
            if (destination == "ManageVat")
            {
                OpenVatDialog();
                return;
            }
            Window? next = destination switch
            {
                "Inventory" => new ProductWindow(),
                // NEW: “Others” submenu routes
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
            var old = app.MainWindow; // pointer to the prev main window to not create a new one each time.

            next.WindowState = WindowState.Maximized;
            app.MainWindow = next;
            next.Show();
            old?.Hide();
        }
        private void OpenVatDialog()
        {
            // Find the existing MainWindow instance (even if it’s hidden).
            var rootMain = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault();

            var dlg = new VatPopup
            {
                Owner = rootMain ?? Application.Current.MainWindow, // fallback if needed
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            if (dlg.Owner != null) dlg.Owner.Opacity = 0.4;
            dlg.ShowDialog();
            if (dlg.Owner != null) dlg.Owner.Opacity = 1.0;

        }

    }
}
