using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;



namespace RestND.Helpers
{
    public class EmployeeValidator
    {

        public static bool isNameValid(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string pattern = @"^[A-Za-z]{2,50}$";
            return Regex.IsMatch(name, pattern);
        }

        public static bool isIDValid(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            string pattern = @"^\d{9}$";
            return Regex.IsMatch(id, pattern);

        }
        public static bool isPosValid(string Employee_Pos)
        {
            string waiterPos = "waiter";
            string managerPos = "manager";
            string shift_Manager = "shift manager ";


            return Employee_Pos == waiterPos ||
                   Employee_Pos == managerPos ||
                   Employee_Pos == shift_Manager;



        }
        public static bool isEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool isPasswordValid(string password)
        {

            if (string.IsNullOrEmpty(password)) return false;

            if (password.Length < 6 || password.Length > 12)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            return hasUpper && hasLower;


        }


    }
}
