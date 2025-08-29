using RestND.MVVM.View.Windows;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RestND.MVVM.View.UserControls
{
    public partial class ReturnBtn : UserControl
    {
        public ReturnBtn()
        {
            InitializeComponent();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var hostWindow = Window.GetWindow(this);
            if (hostWindow == null) return;

            if (hostWindow.Owner != null)
            {
                hostWindow.Owner.Show();
                hostWindow.Close();
                return;
            }
            else
            {
                // Fallback: find an existing MainWindow instance (do NOT create a new one)
                var existingMain = Application.Current.Windows
                    .OfType<MainWindow>()
                    .FirstOrDefault() ?? Application.Current.MainWindow as MainWindow;

                if (existingMain != null)
                {
                    existingMain.WindowState = WindowState.Maximized;
                    existingMain.Show();
                }
                // If existingMain is null, your app likely closed MainWindow earlier.
                // In that case you *could* create a new one, but better to avoid ever closing it.
            }

            hostWindow.Hide();
        }
    }
}
