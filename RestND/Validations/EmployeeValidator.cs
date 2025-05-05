using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RestND.MVVM.Model.Employees;
using RestND.Data;

namespace RestND.Validations
{
    public static class EmployeeValidator
    {
        public static Dictionary<string, List<string>> ValidateFields(Employee employee, List<Employee> existingEmployees, bool checkEmailExists = true, bool checkIdExists = true)
        {
<<<<<<< HEAD
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
=======
            var errors = new Dictionary<string, List<string>>();

            // Employee ID validation
            if (employee.Employee_ID <= 0)
>>>>>>> 0f9430044a1547d6411c3a65439a5284440b0196
            {
                AddError(errors, nameof(employee.Employee_ID), "Employee ID must be greater than 0!");
            }
<<<<<<< HEAD
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
=======

            if (checkIdExists && existingEmployees.Any(e => e.Employee_ID == employee.Employee_ID))
>>>>>>> 0f9430044a1547d6411c3a65439a5284440b0196
            {
                AddError(errors, nameof(employee.Employee_ID), "Employee with this ID already exists!");
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(employee.Email))
            {
                AddError(errors, nameof(employee.Email), "Email is required!");
            }
            else
            {
                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(employee.Email, emailPattern))
                {
                    AddError(errors, nameof(employee.Email), "Invalid email format!");
                }

                if (checkEmailExists && existingEmployees.Any(e => e.Email.Equals(employee.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    AddError(errors, nameof(employee.Email), "Employee with this email already exists!");
                }
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(employee.Password))
            {
                AddError(errors, nameof(employee.Password), "Password is required!");
            }
            else
            {
                if (employee.Password.Length < 6 || employee.Password.Length > 12)
                {
                    AddError(errors, nameof(employee.Password), "Password must be between 6 and 12 characters!");
                }

                bool hasUpper = employee.Password.Any(char.IsUpper);
                bool hasLower = employee.Password.Any(char.IsLower);
                if (!(hasUpper && hasLower))
                {
                    AddError(errors, nameof(employee.Password), "Password must contain both uppercase and lowercase letters!");
                }
            }

            return errors;
        }

        // Adds an error to the dictionary
        private static void AddError(Dictionary<string, List<string>> dict, string key, string message)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<string>();
            dict[key].Add(message);
        }
    }
}
