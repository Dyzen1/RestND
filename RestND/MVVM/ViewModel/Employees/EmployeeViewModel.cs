using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.Validations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class EmployeeViewModel : ObservableObject
    {
        #region Services & Validators

        private readonly EmployeeServices _employeeService = new();
        private readonly RoleServices _roleService = new();
        private readonly EmployeeValidator _validator = new();

        #endregion

        #region SignalR Hub

        private readonly HubConnection _hub = App.EmployeeHub; // define App.EmployeeHub similar to App.DishHub/App.InventoryHub

        #endregion

        #region Observable Properties

        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee newEmployee = new();
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private ObservableCollection<Role> roles = new();
        [ObservableProperty] private string formErrorMessage;

        #endregion

        #region Events

        // Raised to let the Window close itself (Edit/Add dialogs)
        public event Action? RequestClose;

        #endregion

        #region Constructor

        public EmployeeViewModel()
        {
            // Listen for real-time updates from other clients
            _hub.On<Employee, string>("ReceiveEmployeeUpdate", (employee, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var match = Employees.FirstOrDefault(e => e.Employee_ID == employee.Employee_ID);

                    switch (action)
                    {
                        case "add":
                            if (match == null)
                            {
                                Employees.Add(employee);
                            }
                            break;

                        case "update":
                            if (match != null)
                            {
                                match.Employee_Name = employee.Employee_Name;
                                match.Employee_LastName = employee.Employee_LastName;
                                match.Employee_Role = employee.Employee_Role;
                                match.Email = employee.Email;
                                match.Is_Active = employee.Is_Active;
                            }
                            break;

                        case "delete":
                            if (match != null)
                                Employees.Remove(match);
                            break;
                    }
                });
            });

            LoadEmployees();
            LoadRoles();
        }

        #endregion

        #region Load Commands

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

        #endregion

        #region Relay Commands

        [RelayCommand]
        private async Task AddEmployee(string password)
        {
            // keep NewEmployee.Password in the model (validator expects it)
            NewEmployee.Password = password ?? string.Empty;

            if (!_validator.ValidateForAdd(NewEmployee, out var err))
            {
                FormErrorMessage = err;
                return;
            }

            if (_employeeService.Add(NewEmployee))
            {
                // Broadcast add to all clients
                await _hub.SendAsync("NotifyEmployeeUpdate", NewEmployee, "add");


                NewEmployee = new Employee();
                FormErrorMessage = string.Empty;

                RequestClose?.Invoke();
            }
       
        }

        [RelayCommand]
        private async Task UpdateEmployee()
        {
            if (SelectedEmployee is null)
            {
                FormErrorMessage = "No employee selected.";
                return;
            }

            if (!_validator.ValidateForUpdate(SelectedEmployee, out var err))
            {
                FormErrorMessage = err;
                return;
            }

            if (_employeeService.Update(SelectedEmployee))
            {
                // Broadcast update to all clients
                await _hub.SendAsync("NotifyEmployeeUpdate", SelectedEmployee, "update");

                FormErrorMessage = string.Empty;
                RequestClose?.Invoke();
            }
        
        }

        [RelayCommand]
        private async Task DeleteEmployeeAsync()
        {
            if (SelectedEmployee is null)
            {
                FormErrorMessage = "No employee selected.";
                return;
            }

            if (_employeeService.Delete(SelectedEmployee))
            {
                // Broadcast delete to all clients
                await _hub.SendAsync("NotifyEmployeeUpdate", SelectedEmployee, "delete");

                SelectedEmployee = null;
            }
     
        }

        #endregion
    }
}
