using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View
{
    public partial class DishWindow : Window
    {
        public DishWindow()
        {
            InitializeComponent();
            this.DataContext = new DishViewModel();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            mainWindow.Show();
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //close + minimize + maximize window.
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            //Application.Current.Shutdown(); - if we want the app to totally close. 
        }
    }
}
