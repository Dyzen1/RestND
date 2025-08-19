using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.Validations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel
{
    public partial class EmployeeViewModel : ObservableObject
    {
        private readonly EmployeeServices _employeeService = new();
        private readonly RoleServices _roleService = new();
        private readonly EmployeeValidator _validator = new();

        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee newEmployee = new();
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private ObservableCollection<Role> roles = new();

        [ObservableProperty] private string formErrorMessage;

        // Raised to let the Window close itself (Edit/Add dialogs)
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

        // Add new employee; pass password from the Add dialog (e.g., CommandParameter)
        [RelayCommand]
        private async Task AddEmployee(string password)
        {
            // keep NewEmployee.Password in the model (validator expects it)
            NewEmployee.Password = password ?? string.Empty;

            if (!_validator.ValidateForAdd(NewEmployee, out var err))
            {
                FormErrorMessage = err;
                await Task.CompletedTask;
                return;
            }

            // email & id uniqueness already validated; extra defensive checks optional
            if (_employeeService.Add(NewEmployee))
            {
                Employees.Add(NewEmployee);
                NewEmployee = new Employee();
                FormErrorMessage = string.Empty;

                // Close the Add dialog (if any listener is attached)
                RequestClose?.Invoke();
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
            if (SelectedEmployee is null)
            {
                FormErrorMessage = "No employee selected.";
                await Task.CompletedTask;
                return;
            }

            // On update: ID format check but no uniqueness; email must be unique excluding self.
            if (!_validator.ValidateForUpdate(SelectedEmployee, out var err))
            {
                FormErrorMessage = err;
                await Task.CompletedTask;
                return;
            }

            if (_employeeService.Update(SelectedEmployee))
            {
                LoadEmployees();
                FormErrorMessage = string.Empty;

                // Close the Edit dialog
                RequestClose?.Invoke();
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
            if (SelectedEmployee is null)
            {
                FormErrorMessage = "No employee selected.";
                await Task.CompletedTask;
                return;
            }

            if (_employeeService.Delete(SelectedEmployee))
            {
                LoadEmployees();
                SelectedEmployee = null;

                // Optionally close a dialog after delete
                RequestClose?.Invoke();
            }
            else
            {
                FormErrorMessage = "Failed to delete employee.";
            }

            await Task.CompletedTask;
        }
    }
}
