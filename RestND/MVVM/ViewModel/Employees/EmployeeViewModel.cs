using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;

using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Orders;
using RestND.utilities;
using RestND.Validations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel
{
    public partial class EmployeeViewModel : ObservableObject, IDisposable
    {
        #region Services & Validators
        private readonly EmployeeServices _employeeService = new();
        private readonly RoleServices _roleService = new();
        private readonly EmployeeValidator _validator = new();
        private readonly OrderServices _orderSvc = new();


        [ObservableProperty]
        private ObservableCollection<Order> employeeOrdersInProgress = new();

        [ObservableProperty]
        private ObservableCollection<Order> employeeOrdersFinished = new();
        #endregion

        #region SignalR Hub (employees only)
        private readonly HubConnection _hub = App.EmployeeHub;
        private IDisposable? _empHandler;
        #endregion

        #region Observable Properties
        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee newEmployee = new();
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private ObservableCollection<Role> roles = new();
        [ObservableProperty] private string formErrorMessage;
        #endregion

        public event Action? RequestClose;

        #region ctors
        public EmployeeViewModel()
        {
            LoadEmployees();
            LoadRoles();
            RegisterMessageHandlers(); // listen to role changes (in-process)
        }
        #endregion

        #region SignalR
        // Call once after hub is started
        public void RegisterHubHandlers()
        {
            _empHandler = _hub.On<Employee, string>("ReceiveEmployeeUpdate", (employee, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var match = Employees.FirstOrDefault(e => e.Employee_ID == employee.Employee_ID);
                    switch (action)
                    {
                        case "add":
                            if (match == null) Employees.Add(employee);
                            break;

                        case "update":
                            if (match != null)
                            {
                                match.Employee_Name = employee.Employee_Name;
                                match.Employee_LastName = employee.Employee_LastName;
                                match.Employee_Role = employee.Employee_Role;
                                match.Is_Active = employee.Is_Active;
                            }
                            break;

                        case "delete":
                            if (match != null) Employees.Remove(match);
                            break;
                    }
                });
            });
        }

        public void UnregisterHubHandlers()
        {
            _empHandler?.Dispose();
            _empHandler = null;
        }

        // Messenger subscription: reacts to role changes from RoleViewModel
        private void RegisterMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<RoleChangedMessage>(this, (_, msg) =>
            {
                var (role, action) = msg.Value;

                // Keep role picker fresh
                var existing = Roles.FirstOrDefault(r => r.Role_ID == role.Role_ID);
                switch (action)
                {
                    case "add":
                        if (existing == null) Roles.Add(role);
                        break;

                    case "update":
                        if (existing != null)
                        {
                            existing.Role_Name = role.Role_Name;
                            existing.Permissions = role.Permissions;
                            existing.Is_Active = role.Is_Active;
                        }
                        break;

                    case "delete":
                        if (existing != null) Roles.Remove(existing);
                        break;
                }

                // Patch employees referencing this role (so UI refreshes immediately)
                foreach (var emp in Employees.Where(e => e.Employee_Role?.Role_ID == role.Role_ID).ToList())
                {
                    emp.Employee_Role = new Role
                    {
                        Role_ID = role.Role_ID,
                        Role_Name = role.Role_Name,
                        Permissions = role.Permissions,
                        Is_Active = role.Is_Active
                    };
                }

                // OPTIONAL: if you keep a global auth context for the logged-in user,
                // refresh permission-gated UI here.
                // AuthContext.ApplyRoleUpdate(role);
            });
        }
        #endregion

        #region Commands
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

        [RelayCommand]
        private async Task AddEmployee(string password)
        {
            NewEmployee.Password = password ?? string.Empty;

            if (!_validator.ValidateForAdd(NewEmployee, out var err))
            {
                FormErrorMessage = err;
                return;
            }

            if (_employeeService.Add(NewEmployee))
            {
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
                await _hub.SendAsync("NotifyEmployeeUpdate", SelectedEmployee, "delete");
                SelectedEmployee = null;
            }
        }
        partial void OnSelectedEmployeeChanged(Employee? value)
        {
            LoadEmployeeOrders();
        }

        private void LoadEmployeeOrders()
        {
            EmployeeOrdersInProgress.Clear();
            EmployeeOrdersFinished.Clear();

            if (SelectedEmployee == null)
                return;

            var orders = _orderSvc.GetOrdersByEmployeeId(SelectedEmployee.Employee_ID);

            foreach (var order in orders)
            {
                if (order.Is_Active)
                    EmployeeOrdersInProgress.Add(order);
                else
                    EmployeeOrdersFinished.Add(order);
            }
        }
        public void Dispose() => UnregisterHubHandlers();
        #endregion
    }
}
