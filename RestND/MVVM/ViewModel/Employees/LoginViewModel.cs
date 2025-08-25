﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Security;
using System;

namespace RestND.MVVM.ViewModel.Employees
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly LoginServices _loginServices = new();

        public event Action? LoginSucceeded;
        public event Action<string>? LoginFailed;

        [ObservableProperty] private int employeeId;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string? errorMessage;
        [ObservableProperty] private Employee? employee;
        [ObservableProperty] private bool success;

        partial void OnSuccessChanged(bool value)
        {
            if (value)
            {
                ErrorMessage = null;
                LoginSucceeded?.Invoke();
            }
        }

        [RelayCommand]
        public void Login()
        {
            ErrorMessage = null;

            if (EmployeeId <= 0 || string.IsNullOrWhiteSpace(Password))
            {
                Success = false;
                ErrorMessage = "Please enter a valid ID and password.";
                LoginFailed?.Invoke(ErrorMessage ?? "");
                return;
            }

            var emp = _loginServices.AuthenticateById(EmployeeId, Password);
            if (emp is null)
            {
                Success = false;
                ErrorMessage = "Invalid ID or password.";
                LoginFailed?.Invoke(ErrorMessage ?? "");
                return;
            }

            Employee = emp;

            // 🔐 Set global auth state (fires AuthContext.SignedIn)
            AuthContext.SignIn(emp);

            Success = true; // triggers LoginSucceeded
        }
    }
}
