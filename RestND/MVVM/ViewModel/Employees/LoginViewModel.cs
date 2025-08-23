using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.Excel;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using System;

namespace RestND.MVVM.ViewModel.Employees
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly LoginServices _loginServices;

        public LoginViewModel()
        {
            _loginServices = new LoginServices();
        }

        // Events (optional but useful if the Window subscribes to close on success)
        public event Action? LoginSucceeded;
        public event Action<string>? LoginFailed;

        [ObservableProperty]
        private string password = string.Empty;

        // Your XAML binds to Id (int). If you prefer text parsing, switch to IdText (string).
        [ObservableProperty]
        private int id;

        [ObservableProperty]
        private Employee? employee;

        [ObservableProperty]
        private bool success;

        // <-- This is what your XAML needs
        [ObservableProperty]
        private string? errorMessage;

        partial void OnSuccessChanged(bool value)
        {
            if (value)
            {
                // Clear any previous error and notify
                ErrorMessage = null;
                LoginSucceeded?.Invoke();
            }
        }

        [RelayCommand]
        private void GetUserPassword()
        {
            // Clear previous error
            errorMessage = null;

            // Validate (add your own checks as needed)
            if (Id <= 0 || string.IsNullOrWhiteSpace(Password))
            {
                Success = false;
                ErrorMessage = "Please enter a valid ID and password.";
                LoginFailed?.Invoke(ErrorMessage);
                return;
            }

            Success = _loginServices.ValidateLogin(Id, Password);

            if (!Success)
            {
                ErrorMessage = "Invalid ID or password.";
                LoginFailed?.Invoke(ErrorMessage);
            }
        }
    }
}
