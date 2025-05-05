using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using RestND.Validations;
using RestND.MVVM.Model.Employees;
using RestND.Data;
using System.Data;



namespace RestND.Helpers
{
    public class EmployeeValidator : GeneralValidations
    {
        Employee employee;
        public EmployeeValidator(Employee employee)
        {
            this.employee = employee;
        }
        public bool employeeID_Validation(out string errorMessage)
        {
            errorMessage = string.Empty;
            int id = employee.Employee_ID;
            if(string.IsNullOrEmpty(employee.Employee_ID.ToString()))
            {
                errorMessage = "Insert ID!";
                return false;
            }
            EmployeeServices employeeServices = new EmployeeServices();
            List<Employee> employees = employeeServices.GetAll();
            if (!employees.Any(employee => employee.Employee_ID == id))
            {
                errorMessage = "Employee with this ID already exists!";
                return false;
            }
            string pattern = @"^\d{9}$";
            if (!Regex.IsMatch(id.ToString(), pattern))
            {
                errorMessage = "Invalid ID!";
                return false;
            }
            return true;
        }
        public bool employeeEmail_Validation(out string errorMessage)
        {
            errorMessage = string.Empty;
            string email = employee.Email;
            if (string.IsNullOrEmpty(employee.Email))
            {
                errorMessage = "Insert email!";
                return false;
            }
            EmployeeServices employeeServices = new EmployeeServices();
            List<Employee> employees = employeeServices.GetAll();
            if (!employees.Any(employee => employee.Email == email))
            {
                errorMessage = "Employee with this email already exists!";
                return false;
            }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if(!Regex.IsMatch(email, pattern))
            {
                errorMessage = "Invalid email!";
                return false;
            }
            return true;
        }

        public bool employeePassword_Validation(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(employee.Password))
            {
                errorMessage = "Insert password!";
                return false;
            }

            if (employee.Password.Length < 6 || employee.Password.Length > 12)
                return false;

            bool hasUpper = employee.Password.Any(char.IsUpper);
            bool hasLower = employee.Password.Any(char.IsLower);
            return hasUpper && hasLower;

        }
    

    }
}
