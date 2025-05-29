using RestND.MVVM.ViewModel;
using System.Windows;

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
            mainWindow.Show();
            this.Close();
        }

        private void AddTableBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
