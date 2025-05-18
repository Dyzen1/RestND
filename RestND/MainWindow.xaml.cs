using RestND.MVVM.View;
using RestND.MVVM.View.UserControls;
using RestND.MVVM.View.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace RestND
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            sideBar.ButtonClicked += SideBar_ButtonClicked;

        }

        private void SideBar_ButtonClicked(string destination)
        {
            switch (destination)
            {
                case "Inventory":
                    OpenInventory();
                    break;
                case "Orders":
                    OpenOrders();
                    break;
                case "Dishes":
                    OpenDishes();
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
        private void OpenInventory()
        {
            var inventoryWindow = new ProductWindow();
            {
                WindowState = WindowState.Maximized;
            }
            inventoryWindow.Show();
            this.Close();
        }
        private void OpenOrders()
        {
            var ordersWindow = new OrderWindow();
            {
                WindowState = WindowState.Maximized;
            }
            ordersWindow.Show();
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
        //private void OpenReports()
        //{
        //    var dishesWindow = new DishWindow();
        //    {
        //        WindowState = WindowState.Maximized;
        //    }
        //    dishesWindow.Show();
        //    this.Close();
        //}
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
