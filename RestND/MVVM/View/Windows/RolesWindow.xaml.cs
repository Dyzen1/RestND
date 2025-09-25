using RestND.MVVM.Model.Employees;
using RestND.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class RolesWindow : Window
    {
        public RolesWindow()
        {
            InitializeComponent();
            this.DataContext = new RoleViewModel();
            DataContextChanged += RolesWindow_DataContextChanged;
            // Hook roles search bar
            RolesSearch.SearchTextChanged += (s, text) => ApplyProductFilter(text);
            // Initial filter after the view is ready
            Loaded += (_, __) =>
            {
                ApplyProductFilter(RolesSearch.SearchText ?? string.Empty);
            };
        }

        //Search bar Filter: roles selections by Role_Name
        private void ApplyProductFilter(string searchText)
        {
            var vm = DataContext as RoleViewModel;
            var items = vm?.Roles;   // ObservableCollection<SelectableProduct>
            if (items is null) return;

            var view = CollectionViewSource.GetDefaultView(items);
            if (view is null) return;

            var q = (searchText ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(q))
            {
                view.Filter = null;
                view.Refresh();
                return;
            }

            view.Filter = o =>
            {
                var i = o as Role;
                return !string.IsNullOrEmpty(i?.Role_Name)
                    && i.Role_Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
            };
            view.Refresh();
        }

        private void RolesWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is RoleViewModel oldVm)
                oldVm.RequestClose -= Vm_RequestClose;

            if (e.NewValue is RoleViewModel newVm)
                newVm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose()
        {
            try { DialogResult = true; } catch { /* non-modal */ }
            Close();
        }

        private void CreateRole_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is RoleViewModel vm)
            {
                // Call the VM's AddRole command explicitly (click event as requested)
                vm.AddRoleCommand.Execute(null);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
