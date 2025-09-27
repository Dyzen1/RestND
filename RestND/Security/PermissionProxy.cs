// RestND.MVVM.Model.Security.PermissionProxy
using RestND.MVVM.Model.Employees;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RestND.MVVM.Model.Security
{
    public sealed class PermissionProxy : INotifyPropertyChanged
    {
        private static readonly Lazy<PermissionProxy> _inst = new(() => new PermissionProxy());
        public static PermissionProxy Instance => _inst.Value;

        private PermissionProxy()
        {
            // Refresh bindings whenever auth changes:
            AuthContext.PermissionsChanged += (_, __) => RaiseAll();
            AuthContext.SignedIn += _ => RaiseAll();
            AuthContext.SignedOut += () => RaiseAll();
        }

        // Map pages/features to flags in your AppPermission enum:
        public bool CanInventory => AuthContext.Has(AppPermission.Inventory);
        public bool CanEmployees => AuthContext.Has(AppPermission.Employees);
        public bool CanDishes => AuthContext.Has(AppPermission.Dishes);
        public bool CanReports => AuthContext.Has(AppPermission.Reports);
        public bool CanOther => AuthContext.Has(AppPermission.Other);
        public bool CanTables => AuthContext.Has(AppPermission.Tables);
        public bool CanSoftDrinks => AuthContext.Has(AppPermission.SoftDrinks);

        public event PropertyChangedEventHandler? PropertyChanged;
        private void Raise([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        private void RaiseAll()
        {
            Raise(nameof(CanInventory));
            Raise(nameof(CanEmployees));
            Raise(nameof(CanDishes));
            Raise(nameof(CanReports));
            Raise(nameof(CanOther));
            Raise(nameof(CanTables));
            Raise(nameof(CanSoftDrinks));
        }
    }
}
