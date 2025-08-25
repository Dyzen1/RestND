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

        public static void SignIn(Employee user)
        {
            CurrentUser = user;
            CurrentPermissions = user?.Employee_Role?.Permissions ?? AppPermission.None;
            SignedIn?.Invoke(user);
        }

        public static void SignOut()
        {
            CurrentUser = null;
            CurrentPermissions = AppPermission.None;
            SignedOut?.Invoke();
        }

        public static bool Has(AppPermission required) => (CurrentPermissions & required) == required;
        public static bool HasAny(AppPermission any) => (CurrentPermissions & any) != 0;
    }
}
