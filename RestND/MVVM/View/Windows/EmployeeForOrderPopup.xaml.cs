using System.Windows;
using RestND.MVVM.ViewModel.Main;

namespace RestND.MVVM.View.Windows
{
    public partial class EmployeeForOrderPopup : Window
    {
        public EmployeeForOrderPopup()
        {
            InitializeComponent();
            DataContext = App.SharedMainVM;
            App.SharedMainVM.ClosePopupAction = this.Close;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
