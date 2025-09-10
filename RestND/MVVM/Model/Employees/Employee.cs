using RestND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Employees
{
    public class Employee
    {
        #region Employee Id
        private int _Employee_ID;
        public int Employee_ID
        {
            get { return _Employee_ID; }
            set { _Employee_ID = value; }
        }
        #endregion

        #region Email

        private string? _Email;
        public string? Email
        {
            get { return _Email; }
            set { _Email = value; }
        }


        #endregion

        #region Password
        private string? _Password;
        public string? Password
        {
            get { return _Password; }
            set { _Password = value; }
        }


        #endregion

        #region Is_Active - a property for knowing wheather the employee has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Employee Name
        private string? _Employee_Name;
        public string? Employee_Name
        {
            get { return _Employee_Name; }
            set { _Employee_Name = value; }
        }
        #endregion

        #region Employee last_Name
        private string? _Employee_LastName;
        public string? Employee_LastName
        {
            get { return _Employee_LastName; }
            set { _Employee_LastName = value; }
        }
        #endregion

        #region Employee Role
        private Role? _Employee_Role;
        public Role? Employee_Role
        {
            get { return _Employee_Role; }
            set { _Employee_Role = value; }
        }
        #endregion

        #region Constructor without email and password
        public Employee(int employeeId, string? employeeName, Role? employeeRole,string employeeLastName)
        {
            this.Employee_LastName = employeeLastName;
            this.Employee_ID = employeeId;
            this.Employee_Name = employeeName;
            this.Employee_Role = employeeRole;
            this.Is_Active = true;
        }
        #endregion

        #region Constructor with email and password
        public Employee(int employeeId, string? employeeName, Role? employeeRole, string email, string password,string employeeLastName)
        {
            this.Employee_LastName= employeeLastName;
            this.Email = email;
            this.Password = password;
            this.Employee_ID = employeeId;
            this.Employee_Name = employeeName;
            this.Employee_Role = employeeRole;
            this.Is_Active = true;
        }
        #endregion

        #region Default Constructor
        public Employee(){}
        

        #endregion
    }
}