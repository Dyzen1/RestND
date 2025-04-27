using RestND.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RestND.MVVM.Model
{
    public class Employee
    {
        #region Employee Id
        protected string _Employee_ID;

        public string Employee_ID
        {
            get { return _Employee_ID; }
            set 
            {
                this._Employee_ID = value; 

            }
        }
        #endregion

        #region Employee Name
        protected string _Employee_Name;

        public string Employee_Name
        {
            get { return _Employee_Name; }
            set
            {
                this._Employee_Name = value;



            }

        }
        #endregion

        #region Employee Position
        protected string _Employee_Pos;


        public string Employee_Pos
        {
            get { return _Employee_Pos; }

            set
            {
                this._Employee_Pos = value;

            }
        }
        #endregion

        #region Authorization Status
        private AuthorizationStatus _AuthoStatus;
        public AuthorizationStatus AuthoStatus
        {
            get { return _AuthoStatus; }
            set { _AuthoStatus = value; }
        }
        #endregion

        #region constructor        
        public Employee(string Employee_ID, string Employee_Name,string Employee_Pos)
        {

            this.Employee_ID = Employee_ID;
            this.Employee_Name = Employee_Name;
            this.Employee_Pos = Employee_Pos;

        }
        #endregion














    }




}
