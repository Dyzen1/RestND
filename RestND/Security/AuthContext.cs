// RestND.MVVM.Model.Security.AuthContext
using RestND.MVVM.Model.Employees;
using System;

namespace RestND.MVVM.Model.Security
{
    public static class AuthContext
    {
        public static Employee? CurrentUser { get; private set; }
        public static AppPermission CurrentPermissions { get; private set; } = AppPermission.None;

        public static event Action<Employee>? SignedIn;
        public static event Action? SignedOut;

        // NEW: fire whenever effective permissions change
        public static event EventHandler? PermissionsChanged;

        public static void SignIn(Employee user)
        {
            CurrentUser = user;
            CurrentPermissions = user?.Employee_Role?.Permissions ?? AppPermission.None;
            SignedIn?.Invoke(user);
            PermissionsChanged?.Invoke(null, EventArgs.Empty); // NEW
        }

        public static void SignOut()
        {
            CurrentUser = null;
            CurrentPermissions = AppPermission.None;
            SignedOut?.Invoke();
            PermissionsChanged?.Invoke(null, EventArgs.Empty); // NEW
        }

        // NEW: call this if you edit the role/permissions while logged in
        public static void UpdatePermissions(AppPermission perms)
        {
            CurrentPermissions = perms;
            PermissionsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static bool Has(AppPermission required) => (CurrentPermissions & required) == required;
        public static bool HasAny(AppPermission any) => (CurrentPermissions & any) != 0;
    }
}
