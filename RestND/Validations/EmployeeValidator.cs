using RestND.Data;
using RestND.MVVM.Model.Employees;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestND.Validations
{
    public class EmployeeValidator
    {
        private readonly EmployeeServices _employeeService = new();

        public bool CheckIfNull(Employee employee, out string err)
        {
            err = string.Empty;
            if (employee == null)
            {
                err = "You must choose/create an employee.";
                return false;
            }
            return true;
        }

        public bool ValidId(int id, out string err, bool checkExists = true)
        {
            err = string.Empty;

            if (id <= 0)
            {
                err = "Employee ID must be a valid number greater than 0!";
                return false;
            }

            if (!Regex.IsMatch(id.ToString(), @"^\d{9}$"))
            {
                err = "Invalid ID format. Must be 9 digits.";
                return false;
            }

            if (checkExists && _employeeService.GetAll().Any(e => e.Employee_ID == id))
            {
                err = "Employee with this ID already exists!";
                return false;
            }

            return true;
        }
        public bool ValidEmail(string email, out string err, bool checkExists = true, int? excludeId = null)
        {
            err = string.Empty;
            if (string.IsNullOrWhiteSpace(email))
            {
                err = "Email is required!";
                return false;
            }

            // Require domain extension with 2-4 letters (.com, .net, .org, .co, etc.)
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,4}$"))
            {
                err = "Invalid email format!";
                return false;
            }

            if (checkExists)
            {
                var exists = _employeeService.GetAll()
                    .Any(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                           && (!excludeId.HasValue || e.Employee_ID != excludeId.Value));

                if (exists)
                {
                    err = "Employee with this email already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ValidPassword(string password, out string err)
        {
            err = string.Empty;
            if (string.IsNullOrWhiteSpace(password))
            {
                err = "Password is required!";
                return false;
            }

            if (password.Length < 6 || password.Length > 12)
            {
                err = "Password must be between 6 and 12 characters!";
                return false;
            }

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);

            if (!(hasUpper && hasLower))
            {
                err = "Password must contain both uppercase and lowercase letters!";
                return false;
            }

            return true;
        }

        public bool ValidNames(string firstName, string lastName, out string err)
        {
            err = string.Empty;

            if (string.IsNullOrWhiteSpace(firstName))
            {
                err = "First name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                err = "Last name is required.";
                return false;
            }

            bool IsOk(string s) => s.All(ch => char.IsLetter(ch) || ch == ' ' || ch == '-' || ch == '\'' || ch == '’');

            if (!IsOk(firstName))
            {
                err = "First name can contain letters, spaces, - or ’ only.";
                return false;
            }

            if (!IsOk(lastName))
            {
                err = "Last name can contain letters, spaces, - or ’ only.";
                return false;
            }

            return true;
        }

        public bool ValidRole(Role role, out string err)
        {
            err = string.Empty;
            if (role == null)
            {
                err = "Please select a role.";
                return false;
            }
            return true;
        }

        // Full-form helpers
        public bool ValidateForAdd(Employee e, out string err)
        {
            if (!CheckIfNull(e, out err)) return false;
            if (!ValidNames(e.Employee_Name, e.Employee_LastName, out err)) return false;
            if (!ValidRole(e.Employee_Role, out err)) return false;
            if (!ValidId(e.Employee_ID, out err, checkExists: true)) return false;
            if (!ValidEmail(e.Email, out err, checkExists: true)) return false;
            if (!ValidPassword(e.Password, out err)) return false;

            return true;
        }

        public bool ValidateForUpdate(Employee e, out string err)
        {
            if (!CheckIfNull(e, out err)) return false;
            if (!ValidNames(e.Employee_Name, e.Employee_LastName, out err)) return false;
            if (!ValidRole(e.Employee_Role, out err)) return false;
            if (!ValidId(e.Employee_ID, out err, checkExists: false)) return false;
            if (!ValidEmail(e.Email, out err, checkExists: true, excludeId: e.Employee_ID)) return false;

            // Password optional on update
            if (!string.IsNullOrWhiteSpace(e.Password))
                if (!ValidPassword(e.Password, out err)) return false;

            err = string.Empty;
            return true;
        }
    }
}
