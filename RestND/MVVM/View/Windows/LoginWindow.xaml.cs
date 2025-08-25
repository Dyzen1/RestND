using System.Windows;
using System.Windows.Input;
using RestND.MVVM.ViewModel.Employees;

namespace RestND.MVVM.View.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // close the window when VM reports success (pure MVVM close pattern)
            Loaded += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                {
                    vm.LoginSucceeded += () => Dispatcher.Invoke(Close);
                    vm.LoginFailed += _ => { /* Error shown via binding */ };
                }
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
                vm.Password = PasswordBox.Password ?? string.Empty;
        }

        // let Enter inside the PasswordBox run the command as well
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is LoginViewModel vm)
            {
                vm.LoginCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ask your administrator to reset your password.");
        }
    }
}
