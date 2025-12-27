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
            _mainVM.ClosePopupAction = () => this.Close();
        }

        private void AddTableBtn_Click(object sender, RoutedEventArgs e)
        {
            // close popup first (instant UI response)
            Close();

            // run AddTable without blocking the popup from closing
            _ = _mainVM.AddTable();
        }

    }
}
