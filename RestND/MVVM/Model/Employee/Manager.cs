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
        #region Email

        private string _Email;

        public string Email
        {
            get { return _Email; }
            set
            {
                this._Email = value;
            }
        }



        #endregion

        #region Password

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                this._Password = value;
            }
        }
        #endregion

        #region constructor
        public Manager(string Employee_ID, string Employee_Name, string Employee_Pos, string email, string pass)
           : base(Employee_ID, Employee_Name, Employee_Pos)

        {


            Email =email;
            Password = pass;

        }
        #endregion


 


    }  
}   

