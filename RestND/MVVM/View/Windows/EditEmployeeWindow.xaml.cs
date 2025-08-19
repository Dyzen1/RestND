using System.Windows;
using System.Windows.Input;
using RestND.MVVM.ViewModel; 

namespace RestND.MVVM.View.Windows
{
    public partial class EditEmployeeWindow : Window
    {
        public EditEmployeeWindow()
        {
            InitializeComponent();

            // Subscribe when DataContext is set or changed
            DataContextChanged += EditEmployeeWindow_DataContextChanged;
        }

        private void EditEmployeeWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EmployeeViewModel oldVm)
                oldVm.RequestClose -= Vm_RequestClose;

            if (e.NewValue is EmployeeViewModel newVm)
                newVm.RequestClose += Vm_RequestClose;
        }

        private void Vm_RequestClose()
        {
            
            if (IsLoaded)
            {
                try { DialogResult = true; } catch {  }
                Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try { DialogResult = false; } catch { /* not dialog */ }
            Close();
        }
    }
}
