
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
            OpenPopup();
        }


        ///// METHODS FOR OPENING WINDOWS:
        private void OpenPopup()
        {
            var popupWindow = new TablePopupWindow();
            {
                WindowState = WindowState.Maximized;
            }
            popupWindow.Show();
            this.Close();
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

        private void addTables_Click(object sender, RoutedEventArgs e)
        {

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
