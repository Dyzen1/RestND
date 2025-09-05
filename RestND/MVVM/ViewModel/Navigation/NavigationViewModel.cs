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
            Window? next = destination switch
            {
                "Inventory" => new ProductWindow(),
                "Other" => new OtherWindow(),
                "Dishes" => new DishWindow(),
                "Reports" => new ReportWindow(),
                "Employees" => new EmployeesWindow(),
                "Orders" => new OrderWindow(),
                "Main" => new MainWindow(), 
                _ => null
            };
            if (next == null) return;

            var app = Application.Current;
            var old = app.MainWindow; // pointer to the prev main window to not create a new one each time.

            next.WindowState = WindowState.Maximized;

            app.MainWindow = next;
            next.Show();

            // now closing the main window.
            old?.Hide();
        }
    }
}
