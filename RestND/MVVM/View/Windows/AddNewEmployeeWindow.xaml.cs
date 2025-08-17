using RestND.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class AddNewEmployeeWindow : Window
    {
        public AddNewEmployeeWindow()
        {
            InitializeComponent();

            // Ensure the correct VM is present
            if (DataContext is not EmployeeViewModel vm)
            {
                vm = new EmployeeViewModel();
                DataContext = vm;
            }

            // Optional: close on success if VM raises RequestClose
            vm.RequestClose -= OnRequestClose;
            vm.RequestClose += OnRequestClose;
        }

        private void OnRequestClose()
        {
            DialogResult = true; // optional
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // optional
            Close();
        }

        // <<< This is the key fix
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is EmployeeViewModel vm)
                vm.AddEmployeeCommand.Execute(pwd.Password);
        }
    }
}
