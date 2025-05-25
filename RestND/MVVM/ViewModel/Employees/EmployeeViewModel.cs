using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.Validations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RestND.MVVM.ViewModel
{
    // ViewModel for managing employees
    public partial class EmployeeViewModel : ObservableObject
    {
        #region Services
        private readonly EmployeeServices _employeeService;
        #endregion

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        [ObservableProperty]
        private Employee newEmployee = new();

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private Dictionary<string, List<string>> employeeValidationErrors = new();

        #endregion

        #region Constructor
        public EmployeeViewModel()
        {
            _employeeService = new EmployeeServices();
        }

        #endregion

        #region On Change
        partial void OnSelectedEmployeeChanged(Employee value)
        {
            UpdateEmployeeCommand.NotifyCanExecuteChanged();
            DeleteEmployeeCommand.NotifyCanExecuteChanged();
            AddEmployeeCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Relay Commands

        [RelayCommand(CanExecute = nameof(CanAddEmployee))]
        private void AddEmployee()
        {
            bool success = _employeeService.Add(NewEmployee);
            if (success)
            {
                Employees.Add(NewEmployee);
                NewEmployee = new Employee();
                EmployeeValidationErrors.Clear();
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyEmployee))]
        private void UpdateEmployee()
        {
            bool success = _employeeService.Update(SelectedEmployee);
            if (success)
            {
                LoadEmployees();
                EmployeeValidationErrors.Clear();
            }
        }

        [RelayCommand(CanExecute = nameof(CanModifyEmployee))]
        private void DeleteEmployee()
        {
            bool success = _employeeService.Delete(SelectedEmployee);
            if (success)
            {
                Employees.Remove(SelectedEmployee);
                SelectedEmployee = null;
                LoadEmployees();
            }
        }

        [RelayCommand]
        private void LoadEmployees()
        {
            Employees = new ObservableCollection<Employee>(_employeeService.GetAll());
        }

        #endregion

        #region Notify CanExecute

        private bool CanAddEmployee()
        {
       
            var errors = EmployeeValidator.ValidateFields(NewEmployee, Employees.ToList());
            return !errors.Any();
        }
        private bool CanModifyEmployee()
        {
            if (SelectedEmployee == null)
                return false;

            var errors = EmployeeValidator.ValidateFields(SelectedEmployee, Employees.Where(e => e.Employee_ID != SelectedEmployee.Employee_ID).ToList());
            return !errors.Any();
        }

        #endregion
    }
}
