
using DocumentFormat.OpenXml.Drawing.Charts;
using RestND.MVVM.View.Windows;
using System;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View
{
    public partial class EmployeesWindow : Window
    {
        public EmployeesWindow()
        {
            InitializeComponent();
        }

        // Matches: MouseLeftButtonDown="Window_MouseLeftButtonDown" in XAML
        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
        private void AddEmployee(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new AddNewEmployeeWindow
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
        private void EditEmployee(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new EditEmployeeWindow
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

        //private void DeleteEmployee(object sender, RoutedEventArgs e)
        //{
        //    Overlay.Visibility = Visibility.Visible;
        //    this.Opacity = 0.4;

        //    var popup = new AddNewEmployeeWindow
        //    {
        //        Owner = this,
        //        WindowStartupLocation = WindowStartupLocation.CenterOwner
        //    };

        //    popup.Closed += (s, args) =>
        //    {
        //        Overlay.Visibility = Visibility.Collapsed;
        //        this.Opacity = 1.0;
        //    };

        //    popup.ShowDialog();
        //}
    }
}
