using RestND.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Employee
    {
        #region Employee Id
<<<<<<< HEAD
        private int _Employee_ID;
        public int Employee_ID
=======
        private string? _Employee_ID;
        public string? Employee_ID
>>>>>>> 5ead172fc20c05b8cefaf649f8c4749d4aebdaca
        {
            get { return _Employee_ID; }
            set { _Employee_ID = value; }
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

        #region Employee Role
        private Role? _Employee_Role;
        public Role? Employee_Role
        {
            get { return _Employee_Role; }
            set { _Employee_Role = value; }
        }
        #endregion

        #region Constructor
<<<<<<< HEAD
        public Employee(int employeeId, string employeeName, Role employeeRole)
=======
        public Employee(string? employeeId, string? employeeName, Role? employeeRole)
>>>>>>> 5ead172fc20c05b8cefaf649f8c4749d4aebdaca
        {
            Employee_ID = employeeId;
            Employee_Name = employeeName;
            Employee_Role = employeeRole;
        }
        #endregion

        #region Default Constructor
        public Employee(){}
        

        #endregion
    }
}