using Google.Protobuf.WellKnownTypes;
using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class GeneralValidations
    {

        public bool isNameValid(string name, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                errorMessage = "Name cannot be empty.";
                return false;
            }

            string pattern = @"^[A-Za-z]{2,50}$"; //only letters, 2 to 50 characters
            if (!Regex.IsMatch(name, pattern))
            {
                errorMessage = "Name must be 2–50 letters only.";
                return false;
            }
            return true;
        }


         public bool isIdValid(string id, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                errorMessage = "Pleas insert an ID!";
                return false;
            }
            string pattern = @"^\d{9}$";
            if (!Regex.IsMatch(id, pattern))
            {
                errorMessage = "Invalid ID!";
                return false;
            }
            return true;
        }

        public bool IsEmptyField(string input, out string err)
        {
            err = string.Empty;
            if (input == null)
            {
                err = "All fields must be populated.";
                return false;
            }
            return true;
        }

        public bool CheckPosNum(int num, out string err)
        {
            err = string.Empty;
            if (num <= 0)
            {
                err = "Number must be positive.";
                return false;
            }
            return true;
        }

    }
}
