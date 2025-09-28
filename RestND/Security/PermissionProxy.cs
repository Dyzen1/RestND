// RestND.MVVM.Model.Security.PermissionProxy
using RestND.MVVM.Model.Employees;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows; // Dispatcher

namespace RestND.MVVM.Model.Security
{
    public sealed class PermissionProxy : INotifyPropertyChanged
    {
        private static readonly Lazy<PermissionProxy> _inst = new(() => new PermissionProxy());
        public static PermissionProxy Instance => _inst.Value;

        private PermissionProxy()
        {
            // Refresh bindings whenever auth changes (events can be raised off the UI thread)
            AuthContext.PermissionsChanged += (_, __) => RaiseAll();
            AuthContext.SignedIn += _ => RaiseAll();
            AuthContext.SignedOut += () => RaiseAll();
        }

        // === Boolean permission properties for XAML (SideBar IsEnabled) ===
        public bool CanInventory => AuthContext.Has(AppPermission.Inventory);
        public bool CanEmployees => AuthContext.Has(AppPermission.Employees);
        public bool CanDishes => AuthContext.Has(AppPermission.Dishes);
        public bool CanReports => AuthContext.Has(AppPermission.Reports);
        public bool CanOrders => AuthContext.Has(AppPermission.Orders);
        public bool CanTables => AuthContext.Has(AppPermission.Tables);
        public bool CanSoftDrinks => AuthContext.Has(AppPermission.SoftDrinks);
        public bool CanOther => AuthContext.Has(AppPermission.Other);

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Raise([CallerMemberName] string? name = null)
        {
            void fire() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            var disp = Application.Current?.Dispatcher;

            if (disp != null && !disp.CheckAccess())
                disp.Invoke(fire);
            else
                fire();
        }

        private void RaiseAll()
        {
            Raise(nameof(CanInventory));
            Raise(nameof(CanEmployees));
            Raise(nameof(CanDishes));
            Raise(nameof(CanReports));
            Raise(nameof(CanOrders));
            Raise(nameof(CanTables));
            Raise(nameof(CanSoftDrinks));
            Raise(nameof(CanOther));
        }
    }
}
