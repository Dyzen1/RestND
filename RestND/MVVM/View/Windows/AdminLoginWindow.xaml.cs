using RestND.MVVM.ViewModel.Employees;
using System.Windows;
using System.Windows.Controls;

namespace RestND.MVVM.View.Windows
{
    public partial class AdminLoginWindow : Window
    {
        public AdminLoginWindow()
        {
            InitializeComponent();

            Loaded += (_, _) => Owner.Opacity = 0.4;
            Closed += (_, _) => Owner.Opacity = 1.0;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password;
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgot = new ForgotPasswordWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
                // need to set up  datacontext for forgot password window DONNT FORGET !!! 
            };
            forgot.ShowDialog();

        }
    }
}
