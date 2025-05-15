using RestND.MVVM.View;
using System.Windows;
using System.Windows.Input;

namespace RestND
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            sideBar.ButtonClicked += OpenInventory;
            sideBar.ButtonClicked += OpenOrders;
            sideBar.ButtonClicked += OpenDishes;

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
            //Application.Current.Shutdown(); - if we want the app to totally close. 
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

    }
}
