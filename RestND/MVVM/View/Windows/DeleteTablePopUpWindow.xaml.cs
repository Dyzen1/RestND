using System.Windows;
using RestND.MVVM.ViewModel.Main;

namespace RestND.MVVM.View.Windows
{
    public partial class DeleteTablePopUpWindow : Window
    {
        private readonly MainWindowViewModel _mainVM;

        public DeleteTablePopUpWindow()
        {
            InitializeComponent();
            _mainVM = App.SharedMainVM;
            this.DataContext = _mainVM;
        }

        private async void DeleteTableBtn_Click(object sender, RoutedEventArgs e)
        {
            await App.SharedMainVM.DeleteTableAsync();
            this.Close();

        }
    }
}