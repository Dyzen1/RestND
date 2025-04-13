using RestND.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RestND.MVVM.Model
{
    public class Manager : Employee
    {

        public string _Email { get; set; }
        public string _Password { get; private set; }
        public Manager(string Employee_ID, string Employee_Name, string Employee_Pos, string email, string Password)
           : base(Employee_ID, Employee_Name, Employee_Pos)

        {


            _Email =email;
            _Password = Password;

        }


        public override bool Equals(object obj)
        {
            if (obj is Manager other)
                return _Employee_ID == other._Employee_ID;

            return false;

        }
        public override string ToString()
        {
            return "Employee ID :" + _Employee_ID + " Employee Name: " + _Employee_Name + " Employee Position: " + _Employee_Pos;
        }




    }  
}   

