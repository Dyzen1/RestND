using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly EmployeeServices _employeeService = new();
        private readonly RoleServices _roleService = new();

        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee newEmployee = new();
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private ObservableCollection<Role> roles = new();

        // Optional message spot (bind to TextBlock if you want)
        [ObservableProperty] private string formErrorMessage;

        public event Action? RequestClose;

        public EmployeeViewModel()
        {
            LoadEmployees();
            LoadRoles();
        }

        [RelayCommand]
        private void LoadEmployees()
        {
            Employees.Clear();
            foreach (var e in _employeeService.GetAll())
                Employees.Add(e);
        }

        [RelayCommand]
        private void LoadRoles()
        {
            Roles = new ObservableCollection<Role>(_roleService.GetAll());
        }

        // IMPORTANT: Option A — command accepts the password (code-behind passes it)
        [RelayCommand]
        private async Task AddEmployee(string password)
        {
            // minimal sanity checks (no external validator)
            if (string.IsNullOrWhiteSpace(NewEmployee.Employee_Name))
            {
                FormErrorMessage = "First name is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewEmployee.Employee_LastName))
            {
                FormErrorMessage = "Last name is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewEmployee.Email) || !IsValidEmail(NewEmployee.Email))
            {
                FormErrorMessage = "Valid email is required.";
                return;
            }
            if (NewEmployee.Employee_Role is null)
            {
                FormErrorMessage = "Please select a role.";
                return;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                FormErrorMessage = "Password is required.";
                return;
            }
            if (Employees.Any(e => e.Employee_ID == NewEmployee.Employee_ID && e.Employee_ID != 0))
            {
                FormErrorMessage = "Employee ID already exists.";
                return;
            }
            if (Employees.Any(e => string.Equals(e.Email, NewEmployee.Email, StringComparison.OrdinalIgnoreCase)))
            {
                FormErrorMessage = "Email already exists.";
                return;
            }

            NewEmployee.Password = password;

            var ok = _employeeService.Add(NewEmployee);
            if (ok)
            {
                Employees.Add(NewEmployee);
                NewEmployee = new Employee();
                FormErrorMessage = string.Empty;

                RequestClose?.Invoke(); // if the window subscribed, it will close
            }
            else
            {
                FormErrorMessage = "Failed to add employee.";
            }

            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task UpdateEmployee()
        {
            if (SelectedEmployee is null) return;

            if (string.IsNullOrWhiteSpace(SelectedEmployee.Employee_Name) ||
                string.IsNullOrWhiteSpace(SelectedEmployee.Employee_LastName) ||
                string.IsNullOrWhiteSpace(SelectedEmployee.Email) ||
                !IsValidEmail(SelectedEmployee.Email) ||
                SelectedEmployee.Employee_Role is null)
            {
                FormErrorMessage = "Please fill all required fields.";
                return;
            }

            // unique email check (excluding self)
            if (Employees.Any(e =>
                e.Employee_ID != SelectedEmployee.Employee_ID &&
                string.Equals(e.Email, SelectedEmployee.Email, StringComparison.OrdinalIgnoreCase)))
            {
                FormErrorMessage = "Email already exists.";
                return;
            }

            var ok = _employeeService.Update(SelectedEmployee);
            if (ok)
            {
                LoadEmployees();
                FormErrorMessage = string.Empty;
            }
            else
            {
                FormErrorMessage = "Failed to update employee.";
            }

            await Task.CompletedTask;
        }

        [RelayCommand]
        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee is null) return;

            var ok = _employeeService.Delete(SelectedEmployee);
            if (ok)
            {
                LoadEmployees();
                SelectedEmployee = null;
            }
            else
            {
                FormErrorMessage = "Failed to delete employee.";
            }

            await Task.CompletedTask;
        }

        private static bool IsValidEmail(string email)
            => Regex.IsMatch(email ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
