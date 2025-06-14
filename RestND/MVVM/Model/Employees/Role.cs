﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Employees
{
    public class Role
    {
        #region Role Id
        private int _Role_ID;
        public int Role_ID
        {
            get { return _Role_ID; }
            set { _Role_ID = value; }
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

        #region Is_Active - a property for knowing wheather the discount has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region constructor
        public Role(string? roleName, AuthorizationStatus roleAuthorization)
        {
            this.Role_Name = roleName;
            this.Role_Authorization = roleAuthorization;
            this.Is_Active = true;
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
