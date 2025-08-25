using System;

namespace RestND.MVVM.Model.Employees
{
    public class Role
    {
        #region Role Id
        private int _Role_ID;
        public int Role_ID
        {
            get => _Role_ID;
            set => _Role_ID = value;
        }
        #endregion

        #region Role Name
        private string? _Role_Name;
        public string? Role_Name
        {
            get => _Role_Name;
            set => _Role_Name = value;
        }
        #endregion

        #region Permissions (flags)
        private AppPermission _Permissions;
        public AppPermission Permissions
        {
            get => _Permissions;
            set => _Permissions = value;
        }
        #endregion

        #region Is_Active
        private bool _Is_Active;
        public bool Is_Active
        {
            get => _Is_Active;
            set => _Is_Active = value;
        }
        #endregion

        // Display-only for your grid’s read-only cell:
        public string Role_Authorization => Permissions.ToString();

        #region Ctors
        public Role() { }

        public Role(string? roleName, AppPermission permissions, bool isActive = true)
        {
            Role_Name = roleName;
            Permissions = permissions;
            Is_Active = isActive;
        }
        #endregion
    }
}
