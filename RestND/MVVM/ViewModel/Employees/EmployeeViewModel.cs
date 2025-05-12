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

        #region Constructor
        public EmployeeViewModel()
        {
            _employeeService = new EmployeeServices();
            employees = new ObservableCollection<Employee>(_employeeService.GetAll());
        }
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

        #region Commands

        [RelayCommand(CanExecute = nameof(CanAddEmployee))]
        private void AddEmployee()
        {
            employeeValidationErrors = EmployeeValidator.ValidateFields(newEmployee, employees.ToList());

            if (employeeValidationErrors.Any())
                return;

            bool success = _employeeService.Add(newEmployee);
            if (success)
            {
                employees.Add(newEmployee);
                newEmployee = new Employee();
                employeeValidationErrors.Clear();
            }
        }

        private bool CanAddEmployee()
        {
            var errors = EmployeeValidator.ValidateFields(newEmployee, employees.ToList());
            return !errors.Any();
        }

        [RelayCommand(CanExecute = nameof(CanModifyEmployee))]
        private void UpdateEmployee()
        {
            if (selectedEmployee == null)
                return;

            var errors = EmployeeValidator.ValidateFields(selectedEmployee, employees.Where(e => e.Employee_ID != selectedEmployee.Employee_ID).ToList());

            if (errors.Any())
            {
                employeeValidationErrors = errors;
                return;
            }

            bool success = _employeeService.Update(selectedEmployee);
            if (success)
            {
                LoadEmployees();
                employeeValidationErrors.Clear();
            }
        }

        private bool CanModifyEmployee()
        {
            if (selectedEmployee == null)
                return false;

            var errors = EmployeeValidator.ValidateFields(selectedEmployee, employees.Where(e => e.Employee_ID != selectedEmployee.Employee_ID).ToList());
            return !errors.Any();
        }

        [RelayCommand(CanExecute = nameof(CanModifyEmployee))]
        private void DeleteEmployee()
        {
            if (selectedEmployee != null)
            {
                bool success = _employeeService.Delete(selectedEmployee.Employee_ID);
                if (success)
                {
                    employees.Remove(selectedEmployee);
                    selectedEmployee = null;
                }
            }
        }

        [RelayCommand]
        private void LoadEmployees()
        {
            employees = new ObservableCollection<Employee>(_employeeService.GetAll());
        }

        #endregion

        #region Notify CanExecute

        partial void OnNewEmployeeChanged(Employee value)
        {
            AddEmployeeCommand.NotifyCanExecuteChanged();
        }

        partial void OnSelectedEmployeeChanged(Employee value)
        {
            UpdateEmployeeCommand.NotifyCanExecuteChanged();
            DeleteEmployeeCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
