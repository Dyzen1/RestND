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

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.SharedMainVM; // keep shared VM
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

        ///// NAVIGATION: been transferred to Nvigation service for officincy. 

        
    }
}
