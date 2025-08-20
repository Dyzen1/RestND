using System.Windows;
using System.Windows.Controls;
using RestND.MVVM.ViewModel.Employees;

namespace RestND.MVVM.View.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            var vm = new LoginViewModel();
            vm.LoginSucceeded += () => this.DialogResult = true;
            vm.LoginFailed += msg => MessageBox.Show(this, msg, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            this.DataContext = vm;

            Loaded += (_, _) => { if (Owner != null) Owner.Opacity = 0.4; };
            Closed += (_, _) => { if (Owner != null) Owner.Opacity = 1.0; };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm && sender is PasswordBox pb)
                vm.Password = pb.Password;
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgot = new ForgotPasswordWindow { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            forgot.ShowDialog();
        }
    }
}
