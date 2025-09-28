// RestND.MVVM.Model.Security.AuthContext
using RestND.MVVM.Model.Employees;
using System;

namespace RestND.MVVM.Model.Security
{
    public static class AuthContext
    {
        // Keep existing surface so nothing else breaks
        public static Employee? CurrentUser { get; private set; }
        public static AppPermission CurrentPermissions { get; private set; } = AppPermission.None;

        public static event Action<Employee>? SignedIn;
        public static event Action? SignedOut;

        // Fire whenever effective permissions change (login, logout, or role updated)
        public static event EventHandler? PermissionsChanged;

        public static void SignIn(Employee user)
        {
            CurrentUser = user;
            CurrentPermissions = user?.Employee_Role?.Permissions ?? AppPermission.None;

            SignedIn?.Invoke(user);
            PermissionsChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SignOut()
        {
            CurrentUser = null;
            CurrentPermissions = AppPermission.None;

            SignedOut?.Invoke();
            PermissionsChanged?.Invoke(null, EventArgs.Empty);
        }

        // If you want to set explicit flags (rare, but keep it for compatibility)
        public static void UpdatePermissions(AppPermission perms)
        {
            CurrentPermissions = perms;

            // keep the in-memory role in sync if present
            if (CurrentUser?.Employee_Role != null)
                CurrentUser.Employee_Role.Permissions = perms;

            PermissionsChanged?.Invoke(null, EventArgs.Empty);
        }

        // ✅ NEW: call this when a Role entity was updated (via SignalR, messenger, etc.)
        public static void ApplyRoleUpdate(Role updated)
        {
            if (CurrentUser?.Employee_Role?.Role_ID == updated.Role_ID)
            {
                // swap the role object so bindings & comparisons see a new snapshot
                CurrentUser.Employee_Role = new Role
                {
                    Role_ID = updated.Role_ID,
                    Role_Name = updated.Role_Name,
                    Permissions = updated.Permissions,
                    Is_Active = updated.Is_Active
                };

                CurrentPermissions = updated.Permissions;
                PermissionsChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static bool Has(AppPermission required) => (CurrentPermissions & required) == required;
        public static bool HasAny(AppPermission any) => (CurrentPermissions & any) != 0;

        // Optional convenience alias if you like the other naming:
        public static Employee? CurrentEmployee => CurrentUser;
    }
}
