using RestND.MVVM.ViewModel.Employees;
using RestND.MVVM.ViewModel.Main;
using System;
using System.Windows;
using System.Windows.Input;

// NEW: auth usings
using RestND.MVVM.Model.Employees;   // AppPermission
using RestND.MVVM.Model.Security;    // AuthContext

namespace RestND.MVVM.View.Windows
{
    public partial class MainWindow : Window
    {
        public event Action<string> ButtonClicked;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.SharedMainVM; // keep shared VM
            sideBar.ButtonClicked += SideBar_ButtonClicked;
        }

        //// ON CLICK METHODS:
        private void SideBar_ButtonClicked(string destination)
        {
            switch (destination)
            {
                case "Inventory":
                    OpenInventory();
                    break;
                case "OverView":
                    OpenOverView();
                    break;
                case "Dishes":
                    OpenDishes();
                    break;
                case "Reports":
                    OpenReports();
                    break;
                case "Employees":
                    OpenEmployees();
                    break;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        //close + minimize + maximize window.
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenOrders();
        }

        private void AddTable_Click(object sender, RoutedEventArgs e)
        {
            // NEW: gate by Tables permission
            if (!AuthContext.Has(AppPermission.Tables))
            {
                MessageBox.Show("You don't have permission to add tables.");
                return;
            }

            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new TablePopupWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            popup.Closed += (s, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                this.Opacity = 1.0;
            };

            popup.ShowDialog();
        }

        private void EditTable_Click(object sender, RoutedEventArgs e)
        {
            // NEW: gate by Tables permission
            if (!AuthContext.Has(AppPermission.Tables))
            {
                MessageBox.Show("You don't have permission to edit tables.");
                return;
            }

            var popup = new EditTablePopUpWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };  

            this.Opacity = 0.4;
            Overlay.Visibility = Visibility.Visible;

            popup.Closed += (_, _) =>
            {
                this.Opacity = 1.0;
                Overlay.Visibility = Visibility.Collapsed;
            };

            popup.ShowDialog();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataContext = new LoginViewModel()
            };

            this.Opacity = 0.4;
            loginWindow.ShowDialog();
            this.Opacity = 1.0;
        }

        private void DeleteTable_Click(object sender, RoutedEventArgs e)
        {
            // NEW: gate by Tables permission
            if (!AuthContext.Has(AppPermission.Tables))
            {
                MessageBox.Show("You don't have permission to delete tables.");
                return;
            }

            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new DeleteTablePopUpWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            popup.Closed += (s, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                this.Opacity = 1.0;
            };

            popup.ShowDialog();
        }

        ///// NAVIGATION (hide main, set owner on child):

        private void OpenInventory()
        {
            // NEW
            if (!AuthContext.Has(AppPermission.Inventory))
            {
                MessageBox.Show("You don't have permission to open Inventory.");
                return;
            }

            var w = new ProductWindow { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }

        private void OpenOverView()
        {
            // NEW
            if (!AuthContext.Has(AppPermission.OverView))
            {
                MessageBox.Show("You don't have permission to open OverView.");
                return;
            }

            var w = new OverView { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }

        private void OpenDishes()
        {
            // NEW
            if (!AuthContext.Has(AppPermission.Dishes))
            {
                MessageBox.Show("You don't have permission to open Dishes.");
                return;
            }

            var w = new DishWindow { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }

        private void OpenReports()
        {
            // NEW
            if (!AuthContext.Has(AppPermission.Reports))
            {
                MessageBox.Show("You don't have permission to open Reports.");
                return;
            }

            var w = new ReportWindow { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }

        private void OpenOrders()
        {
          

            var w = new OrderWindow { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }

        private void OpenEmployees()
        {
            // NEW
            if (!AuthContext.Has(AppPermission.Employees))
            {
                MessageBox.Show("You don't have permission to open Employees.");
                return;
            }

            var w = new EmployeesWindow { Owner = this, WindowState = WindowState.Maximized };
            w.Show();
            this.Hide();
        }
    }
}
