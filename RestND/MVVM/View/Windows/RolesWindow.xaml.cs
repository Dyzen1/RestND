using RestND.MVVM.ViewModel;
using System;
using System.Windows;
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
