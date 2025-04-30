using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Role
    {
        #region Id
        private int _Role_ID;
        public int Role_ID
        {
            get { return _Role_ID; }
            set { _Role_ID = value; }
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

        #region Role Name
        private string? _Role_Name;
        public string? Role_Name
        {
            get { return _Role_Name; }
            set { _Role_Name = value; }
        }
        #endregion

        #region Role Authorization
        private AuthorizationStatus _Role_Authorization;

        public AuthorizationStatus Role_Authorization
        {
            get { return _Role_Authorization; }
            set { _Role_Authorization = value; }
        }
        #endregion

        #region constructor
        public Role(string? roleName, AuthorizationStatus roleAuthorization)
        {
            Role_Name = roleName;
            Role_Authorization = roleAuthorization;
        }
        #endregion

        #region constructor for email and password
        public Role(string? roleName, AuthorizationStatus roleAuthorization,string? pass,string? email)
        {
            Email = email;
            Password = pass;
            Role_Name = roleName;
            Role_Authorization = roleAuthorization;
        }
        #endregion

        #region Default Constructor
        public Role()
        {
            //default constructor
        }
        #endregion

    }
}
