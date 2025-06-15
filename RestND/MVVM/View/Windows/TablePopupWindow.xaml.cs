using RestND.MVVM.ViewModel.Main;
using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class TablePopupWindow : Window
    {
        private readonly MainWindowViewModel _mainVM;

        public TablePopupWindow()
        {
            InitializeComponent();
            _mainVM = App.SharedMainVM;
            this.DataContext = _mainVM;
        }

        private async void AddTableBtn_Click(object sender, RoutedEventArgs e)
        {
            await _mainVM.AddTableAsync();
            this.Close();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }
    }
}
