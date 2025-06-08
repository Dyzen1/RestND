using DocumentFormat.OpenXml.Drawing.Charts;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View
{
    public partial class DishWindow : Window
    {
        public event Action<string> ButtonClicked;
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

        private void UpdateDishBtn_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as DishViewModel;
            if (vm?.SelectedDish != null)
            {
                var editWindow = new EditDishPopup
                {
                    Owner = this,
                    DataContext = new EditDishViewModel(vm.SelectedDish)
                };

                this.Opacity = 0.4;
                editWindow.ShowDialog();
                this.Opacity = 1.0;

                vm.LoadDishesCommand.Execute(null);
            }
            else
            {
                MessageBox.Show("Please select a dish to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //the popup used for choosing products for a new dish.
        //private void ChoseProductsPopup_Click(object sender, RoutedEventArgs e)
        //{
        //    Overlay.Visibility = Visibility.Visible;

        //    var popup = new AddProductToDishPopup();
        //    popup.Owner = this;
        //    popup.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        //    popup.Closed += (s, args) =>
        //    {
        //        Overlay.Visibility = Visibility.Collapsed;
        //    };

        //    popup.Show();
        //}
    }
}
