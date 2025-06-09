
using RestND.MVVM.ViewModel.Employees;
using System;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class MainWindow : Window
    {
        public event Action<string> ButtonClicked;

        public MainWindow()
        {
            InitializeComponent();
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
            }
        }

        //method for being able to move the window with the mouse. 
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
            if(WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
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
            Overlay.Visibility = Visibility.Visible;

            var popup = new TablePopupWindow();
            popup.Owner = this;
            popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            popup.Closed += (s, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
            };

            popup.Show();
        }


        ///// METHODS FOR OPENING WINDOWS:

        private void OpenInventory()
        {
            var inventoryWindow = new ProductWindow();
            {
                WindowState = WindowState.Maximized;
            }
            inventoryWindow.Show();
            this.Close();
        }

        private void OpenOverView()
        {
            var overView = new OverView();
            {
                WindowState = WindowState.Maximized;
            }
            overView.Show();
            this.Close();
        }

        private void OpenDishes()
        {
            var dishesWindow = new DishWindow();
            {
                WindowState = WindowState.Maximized;
            }
            dishesWindow.Show();
            this.Close();
        }

        private void OpenReports()
        {
            var reportWindow = new ReportWindow();
            {
                WindowState = WindowState.Maximized;
            };
            reportWindow.Show();
            this.Close();
        }

        private void OpenOrders()
        {
            var ordersWindow = new OrderWindow();
            {
                WindowState = WindowState.Maximized;
            };
            ordersWindow.Show();
            this.Close();
        }
        private void AdminLogin_Click(object sender, RoutedEventArgs e)
        {
            OpenAdminLogin();
        }

        private void OpenAdminLogin()
        {
            var loginWindow = new AdminLoginWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                DataContext =  new LoginViewModel()

            };

            this.Opacity = 0.4;
            loginWindow.ShowDialog();
            this.Opacity = 1.0;
        }


        //private void OpenEmployees()
        //{
        //    var dishesWindow = new DishWindow();
        //    {
        //        WindowState = WindowState.Maximized;
        //    }
        //    dishesWindow.Show();
        //    this.Close();
        //}

    }
}
