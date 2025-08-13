using DocumentFormat.OpenXml.Drawing.Charts;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                var editWindow = new EditDishPopup(vm.SelectedDish)
                {
                    Owner = this,
                    DataContext = new EditDishViewModel(vm.SelectedDish)
                };

                this.Opacity = 0.4;
                editWindow.Owner = this;
                editWindow.ShowDialog();
                this.Opacity = 1.0;

                vm.LoadDishesCommand.Execute(null);
            }
            else
            {
                MessageBox.Show("Please select a dish to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
