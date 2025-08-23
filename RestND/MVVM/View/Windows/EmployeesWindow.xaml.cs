
using DocumentFormat.OpenXml.Drawing.Charts;
using RestND.MVVM.View.Windows;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RestND.MVVM.View
{
    public partial class EmployeesWindow : Window
    {
        public EmployeesWindow()
        {
            InitializeComponent();
            EmployeeSearchBar.SearchTextChanged += EmployeeSearchBar_SearchTextChanged;
        }

        // Matches: MouseLeftButtonDown="Window_MouseLeftButtonDown" in XAML
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void AddEmployee(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new AddNewEmployeeWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            popup.Closed += (s, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                this.Opacity = 1.0;
            };

            popup.ShowDialog();
        }
        private void EditEmployee(object sender, RoutedEventArgs e)
        {
            if (DataContext is RestND.MVVM.ViewModel.EmployeeViewModel vm)
            {
                if (vm.SelectedEmployee == null)
                {
                    MessageBox.Show("Please select an employee first.");
                    return;
                }

                // Ensure roles list exists
                if (vm.Roles == null || vm.Roles.Count == 0)
                    vm.LoadRolesCommand.Execute(null);

                // Dim background
                Overlay.Visibility = Visibility.Visible;
                this.Opacity = 0.4;

                var dlg = new EditEmployeeWindow
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = vm
                };

                // Close handler from VM
                void CloseEditOnRequest()
                {
                    if (dlg.IsVisible) dlg.Close();
                }

                // Always clean up + restore UI
                void Cleanup(object? s, EventArgs args)
                {
                    vm.RequestClose -= CloseEditOnRequest;
                    dlg.Closed -= Cleanup;
                    Overlay.Visibility = Visibility.Collapsed;
                    this.Opacity = 1.0;
                }

                vm.RequestClose += CloseEditOnRequest;
                dlg.Closed += Cleanup;

                dlg.ShowDialog(); // modal
            }
        }
        private void OpenRoles_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
            this.Opacity = 0.4;

            var popup = new RolesWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            popup.Closed += (s, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
                this.Opacity = 1.0;

                // Reload roles on close so the new role appears immediately in dropdowns etc.
                if (DataContext is RestND.MVVM.ViewModel.EmployeeViewModel vm)
                    vm.LoadRolesCommand.Execute(null);
            };

            popup.ShowDialog();
        }
        private void EmployeeSearchBar_SearchTextChanged(object sender, string searchText)
        {
            ApplyEmployeeFilter(searchText);
        }



        private void ApplyEmployeeFilter(string searchText)
        {
            if (DataContext is not RestND.MVVM.ViewModel.EmployeeViewModel vm) return;

            var items = vm.Employees;
            if (items is null) return;

            ICollectionView view = CollectionViewSource.GetDefaultView(items);
            if (view == null) return;

            string q = (searchText ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(q))
            {
                view.Filter = null;  // show all
                view.Refresh();
                return;
            }

            view.Filter = o =>
            {
                if (o is not RestND.MVVM.Model.Employees.Employee emp)
                    return false;

                return emp.Employee_ID.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                    || (emp.Employee_Name?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (emp.Employee_LastName?.Contains(q, StringComparison.OrdinalIgnoreCase) ?? false);
            };
            view.Refresh();
        }



    }
}
