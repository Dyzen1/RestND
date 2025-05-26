using RestND.MVVM.View.Windows;
using RestND.MVVM.ViewModel;
using RestND.MVVM.ViewModel.Orders;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class TablePopupWindow : Window
    {
        public TablePopupWindow()
        {
            InitializeComponent();
            this.DataContext = new TableViewModel();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            mainWindow.ShowDialog();
            this.Close();
        }
    }
}
